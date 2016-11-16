using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class RetryRobot
    {
        public int FirstRetryDelayMs { get; set; } = 1000;
        public float DelayMultiplier { get; set; } = 2;
        public int MaxTryCount { get; set; } = 6;
        public IEnumerable<Type> RetryWhenExceptions { get; set; } = new Type[0];
        public Func<Exception, bool> ShouldRetry { get; set; }

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
                catch (Exception e)
                when (ShouldRetry == null
                    ? RetryWhenExceptions.Contains(e.GetType())
                    : ShouldRetry(e)
                    && i < MaxTryCount)
                {
                }
            }
        }

        public void Eventually(Action action)
        {
            Eventually(() => { action(); return 0; });
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
        public Func<string[], int> Main { get; set; }
        public Action<string[]> VoidMain { get; set; }
        public string Command { get; set; }

        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public ConsoleOutput Run(params string[] arguments)
        {
            Console.Write($"{Command} ");
            Console.WriteLine(string.Join(" ", arguments));

            TextWriter consoleOut = Console.Out;
            StringWriter stringOut = new StringWriter();
            Console.SetOut(stringOut);
            try
            {
                int exitCode = 0;
                if (null == VoidMain)
                    exitCode = Main(arguments);
                else
                    VoidMain(arguments);
                var consoleOutput = new ConsoleOutput()
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
}
