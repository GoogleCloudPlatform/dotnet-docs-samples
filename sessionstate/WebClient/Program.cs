// Copyright 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebClient
{
    internal class Options
    {
        [Option('d', "delay", HelpText = "Milliseconds to delay between page fetches.")]
        public int Delay { get; set; } = 1000;

        [Option('c', "clients", HelpText = "Number of HTTP clients.")]
        public int ClientCount { get; set; } = 100;

        [Option('u', "baseUri", Required = true, HelpText = "The base url running the WebApp.")]
        public string BaseUri { get; set; }

        [Option('s', "sizeMultiplier", HelpText = "Multiplier for session value size.")]
        public int SizeMultiplier { get; set; } = 40;
    }

    public class Program
    {
        private static async Task PutValueAsync(HttpClient client, int key, string value)
        {
            var formData = new Dictionary<string, string>()
            {
                ["Key"] = key.ToString(),
                ["Value"] = value,
                ["Silent"] = "true",
            };
            var formContent = new FormUrlEncodedContent(formData);
            await client.PostAsync("Home/S", formContent);
        }

        // Returns the average page fetch time.
        private static async Task<double> TaskMainAsync(Uri baseAddress, int delayInMilliseconds, int sizeMultiplier)
        {
            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer()
            };
            HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress };
            Stopwatch stopwatch = new Stopwatch();
            // Keep our own copy of the session vars to verify that the server
            // is correctly updating them.
            var sessionVars = new string[10];
            // Add 10 session vars:
            for (int i = 0; i < sessionVars.Length; ++i)
            {
                string content = new string((char)('A' + i), sizeMultiplier * (i + 1));
                sessionVars[i] = content;
                stopwatch.Start();
                await PutValueAsync(client, i, content);
                stopwatch.Stop();
                await Task.Delay(delayInMilliseconds);
            }
            // Read and write the session vars a bunch of times.
            uint contentChar = 'A';
            for (int i = 0; i < 50; ++i)
            {
                await Task.Delay(delayInMilliseconds);
                stopwatch.Start();
                int sessionVarId = i % 10;
                if (i % 3 == 0)
                {
                    string content = new string((char)(contentChar), sizeMultiplier * (sessionVarId + 1));
                    sessionVars[sessionVarId] = content;
                    contentChar = contentChar == 'Z' ? 'A' : contentChar + 1;
                    await PutValueAsync(client, sessionVarId, content);
                }
                else
                {
                    var response = await client.GetAsync($"Home/S/{sessionVarId}");
                    var content = await response.Content.ReadAsStringAsync();
                    if (content != sessionVars[sessionVarId])
                    {
                        Console.WriteLine("Expected to find {0} at Home/S/{1}.  Found {2}",
                            sessionVars[sessionVarId], sessionVarId, content);
                        Environment.Exit(-1);
                    }
                }
                stopwatch.Stop();
            }
            return stopwatch.ElapsedMilliseconds / 60.0;
        }

        public static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
              .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts));
        }

        private static int RunOptionsAndReturnExitCode(Options options)
        {
            Uri baseAddress = new Uri(options.BaseUri);
            var stopwatch = new Stopwatch();
            var tasks = new Task<double>[options.ClientCount];
            stopwatch.Start();
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = Task.Run(async () => await TaskMainAsync(baseAddress,
                    options.Delay, options.SizeMultiplier));
            }
            Task.WaitAll(tasks);
            stopwatch.Stop();
            Console.WriteLine("Total elapsed seconds: {0}", stopwatch.ElapsedMilliseconds / 1000.0);
            var averagePageFetchTime = tasks.Select((task) => task.Result).Average();
            Console.WriteLine("Average page fetch time in milliseconds: {0}", averagePageFetchTime);
            return 0;
        }
    }
}