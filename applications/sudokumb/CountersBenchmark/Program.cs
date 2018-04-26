// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sudokumb
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var counterTypes = new[]
            {
                typeof(UnsynchronizedCounter),
                typeof(LockingCounter),
                typeof(InterlockedCounter),
                typeof(ShardedCounter)
            };
            // Record results so we can render them with Visjs.
            List<VisItem> visItems = new List<VisItem>();
            List<VisGroup> visGroups = new List<VisGroup>();
            int groupNumber = 0;
            foreach (var type in counterTypes)
            {
                visGroups.Add(new VisGroup()
                {
                    id = groupNumber,
                    content = type.Name
                });
                visItems.Add(RunBenchmark(1, type, groupNumber++));
            }
            foreach (int taskCount in new int[] { 2, 4, 8, 16 })
            {
                groupNumber = 1;
                foreach (var type in counterTypes.Skip(1))
                {
                    visItems.Add(RunBenchmark(taskCount, type, groupNumber++));
                }
            }
            string indexPath = RenderResults(visItems, visGroups);
            Console.WriteLine(new System.Uri(indexPath).AbsoluteUri);
        }

        private static VisItem RunBenchmark(int taskCount, Type counterType,
            int groupNumber)
        {
            long count = RunBenchmark(taskCount,
                (ICounter)Activator.CreateInstance(counterType));
            return new VisItem()
            {
                x = taskCount,
                y = count,
                group = groupNumber
            };
        }

        private static long RunBenchmark(int taskCount, ICounter counter)
        {
            Console.WriteLine("Running benchmark for {0} with {1} tasks...",
                counter.GetType().FullName, taskCount);
            CancellationTokenSource cancel = new CancellationTokenSource();
            Task[] tasks = new Task[taskCount];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (!cancel.Token.IsCancellationRequested)
                        counter.Increase(1);
                });
            }
            // Run the incrementing tasks for 3 seconds, and print the running
            // count once per second.
            long count = 0;
            for (int i = 0; i < 3; ++i)
            {
                Thread.Sleep(1000);
                count = counter.Count;
                Console.WriteLine(count);
            }
            cancel.Cancel();
            Task.WaitAll(tasks);
            return count;
        }

        private static string RenderResults(IEnumerable<VisItem> items,
            IEnumerable<VisGroup> groups)
        {
            string tempDir = Path.GetTempFileName();
            File.Delete(tempDir);
            Directory.CreateDirectory(tempDir);
            UnpackEmbeddedFile("wwwroot.vis.min.css",
                Path.Combine(tempDir, "vis.min.css"));
            UnpackEmbeddedFile("wwwroot.vis.min.js",
                Path.Combine(tempDir, "vis.min.js"));
            string text = ReadEmbeddedFile("wwwroot.index.html");
            text = text.Replace("ITEMS", JsonConvert.SerializeObject(items));
            text = text.Replace("GROUPS", JsonConvert.SerializeObject(groups));
            string indexHtml = Path.Combine(tempDir, "index.html");
            File.WriteAllText(indexHtml, text);
            return indexHtml;
        }

        private static string ReadEmbeddedFile(string name)
        {
            var embeddedProvider = new EmbeddedFileProvider(
                Assembly.GetEntryAssembly());
            var info = embeddedProvider.GetFileInfo(name);
            var buffer = new byte[info.Length];
            var reader = new StreamReader(info.CreateReadStream());
            return reader.ReadToEnd();
        }

        private static void UnpackEmbeddedFile(string name, string outputPath)
        {
            var embeddedProvider = new EmbeddedFileProvider(
                Assembly.GetEntryAssembly());
            var info = embeddedProvider.GetFileInfo(name);
            var buffer = new byte[info.Length];
            info.CreateReadStream().Read(buffer, 0, buffer.Length);
            File.WriteAllBytes(outputPath, buffer);
        }
    }

    /// <summary>
    /// A Visjs data point.
    /// </summary>
    internal class VisItem
    {
        public int x { get; set; }
        public long y { get; set; }
        public int group { get; set; }
    }

    /// <summary>
    /// A Visjs group definition.
    /// </summary>
    internal class VisGroup
    {
        public int id { get; set; }
        public string content { get; set; }
    }
}