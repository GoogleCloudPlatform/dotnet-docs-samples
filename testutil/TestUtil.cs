/*
 * Copyright (c) 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace GoogleCloudSamples
{
    public class TestUtil
    {
        public static string RandomName()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                string legalChars = "abcdefhijklmnpqrstuvwxyz";
                byte[] randomByte = new byte[1];
                char[] randomChars = new char[20];
                int nextChar = 0;
                while (nextChar < randomChars.Length)
                {
                    rng.GetBytes(randomByte);
                    if (legalChars.Contains((char)randomByte[0]))
                        randomChars[nextChar++] = (char)randomByte[0];
                }
                return new string(randomChars);
            }
        }
    }

    // TODO: Remove this class and use
    //       Transient Fault Handling Application Block:
    //       https://msdn.microsoft.com/en-us/library/dn440719(v=pandp.60).aspx
    public class RetryRobot
    {
        public int FirstRetryDelayMs { get; set; } = 1000;
        public float DelayMultiplier { get; set; } = 2;
        public int MaxTryCount { get; set; } = 7;
        public IEnumerable<Type> RetryWhenExceptions { get; set; } = new Type[0];
        public Func<Exception, bool> ShouldRetry { get; set; }

        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        /// <summary>
        /// Retry action when assertion fails.
        /// </summary>
        /// <param name="func"></param>
        public T Eventually<T>(Func<T> func)
        {
            int delayMs = FirstRetryDelayMs;
            for (int i = 0; ; ++i)
            {
                try
                {
                    return func();
                }
                catch (Exception e) when (ShouldCatch(e) && i < MaxTryCount)
                {
                    int jitteredDelayMs;
                    lock(_lock)
                    {
                        jitteredDelayMs = delayMs/2 + (int)(_random.NextDouble() * delayMs);
                    }
                    Thread.Sleep(jitteredDelayMs);
                    delayMs *= (int)DelayMultiplier;
                }
            }
        }

        public void Eventually(Action action) =>
            Eventually(() =>
            { 
                action();
                return 0; 
            });


        public async Task<T> Eventually<T>(Func<Task<T>> asyncFunc)
        {
            int delayMs = FirstRetryDelayMs;
            for (int i = 0; ; ++i)
            {
                try
                {
                    return await asyncFunc();
                }
                catch (Exception e) when (ShouldCatch(e) && i < MaxTryCount)
                {
                    int jitteredDelayMs;
                    lock (_lock)
                    {
                        jitteredDelayMs = delayMs / 2 + (int)(_random.NextDouble() * delayMs);
                    }
                    await Task.Delay(jitteredDelayMs);
                    delayMs *= (int)DelayMultiplier;
                }
            }
        }

        public async Task Eventually(Func<Task> action)
        {
            await Eventually(async () =>
            { 
                await action();
                return 0; 
            });
        }

        private bool ShouldCatch(Exception e)
        {
            if (ShouldRetry != null)
                return ShouldRetry(e);
            foreach (Type exceptionType in RetryWhenExceptions)
            {
                if (exceptionType.IsAssignableFrom(e.GetType()))
                    return true;
            }
            return false;
        }
    }

    public struct ConsoleOutput
    {
        public int ExitCode;
        public string Stdout;

        public void AssertSucceeded()
        {
            Assert.True(0 == ExitCode, $"Exit code: {ExitCode}\n{Stdout}");
        }
    };

    public class CommandLineRunner
    {
        // Use a lock to protect globally-shared stdout.
        private static readonly object s_lock = new object();

        public Func<string[], int> Main { get; set; }
        public Action<string[]> VoidMain { get; set; }
        public string Command { get; set; }

        /// <summary>Runs executable with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public ConsoleOutput Run(params string[] arguments)
        {
            lock (s_lock)
            {
                Console.Write($"{Command} ");
                Console.WriteLine(string.Join(" ", arguments));

                TextWriter consoleOut = Console.Out;
                ThreadSafeStringWriter stringOut = new ThreadSafeStringWriter();
                Console.SetOut(stringOut);
                try
                {
                    int exitCode = 0;
                    if (null == VoidMain)
                        exitCode = Main(arguments);
                    else
                        VoidMain(arguments);
                    ConsoleOutput consoleOutput = new ConsoleOutput()
                    {
                        ExitCode = exitCode,
                        Stdout = stringOut.ToString()
                    };
                    Console.Write(consoleOutput.Stdout);
                    return consoleOutput;
                }
                finally
                {
                    Console.SetOut(consoleOut);
                }
            }
        }

        public ConsoleOutput RunWithStdIn(string stdIn, params string[] arguments)
        {
            lock (s_lock)
            {
                Console.Write($"{Command} ");
                Console.WriteLine(string.Join(" ", arguments));

                TextWriter consoleOut = Console.Out;
                TextReader consoleIn = Console.In;
                ThreadSafeStringWriter stringOut = new ThreadSafeStringWriter();
                Console.SetOut(stringOut);
                Console.SetIn(new StringReader(stdIn));
                try
                {
                    int exitCode = 0;
                    if (null == VoidMain)
                        exitCode = Main(arguments);
                    else
                        VoidMain(arguments);
                    ConsoleOutput consoleOutput = new ConsoleOutput()
                    {
                        ExitCode = exitCode,
                        Stdout = stringOut.ToString()
                    };
                    Console.Write(consoleOutput.Stdout);
                    return consoleOutput;
                }
                finally
                {
                    Console.SetOut(consoleOut);
                    Console.SetIn(consoleIn);
                }
            }
        }

        internal class ThreadSafeStringWriter : StringWriter
        {
            private readonly object _lock = new object();

            public override void Write(char value)
            {
                lock (_lock)
                {
                    base.Write(value);
                }
            }

            public override void Write(char[] buffer, int index, int count)
            {
                lock (_lock)
                {
                    base.Write(buffer, index, count);
                }
            }

            public override void Write(string value)
            {
                lock (_lock)
                {
                    base.Write(value);
                }
            }

            public override string ToString()
            {
                lock (_lock)
                {
                    return base.ToString();
                }
            }
        }

    }
}