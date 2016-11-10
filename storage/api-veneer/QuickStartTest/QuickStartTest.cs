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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    public class BaseTest
    {
        public struct ConsoleOutput
        {
            public int ExitCode;
            public string Stdout;
        };

        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public static ConsoleOutput Run(params string[] arguments)
        {
            Console.Write("QuickStart.exe ");
            Console.WriteLine(string.Join(" ", arguments));

            using (var output = new StringWriter())
            {
                QuickStart quickStart = new QuickStart(output);
                var consoleOutput = new ConsoleOutput()
                {
                    ExitCode = quickStart.Run(arguments),
                    Stdout = output.ToString()
                };
                Console.Write(consoleOutput.Stdout);
                return consoleOutput;
            }
        }

        protected static void AssertSucceeded(ConsoleOutput output)
        {
            Assert.True(0 == output.ExitCode,
                $"Exit code: {output.ExitCode}\n{output.Stdout}");
        }
    }

    public class BadCommandTests : BaseTest
    {
        [Fact]
        public void TestNoArgs()
        {
            var ran = Run();
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }

        [Fact]
        public void TestBadCommand()
        {
            var ran = Run("throw");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }

        [Fact]
        public void TestMissingDeleteArg()
        {
            var ran = Run("delete");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }
    }

    /// <summary>
    /// Trying to create a bucket for each test will exceed our rate limit.
    /// Therefore, share one bucket across all the tests.
    /// </summary>
    public class BucketFixture : IDisposable
    {
        public BucketFixture()
        {
            BucketName = QuickStartTest.CreateRandomBucket();
        }
        public void Dispose()
        {
            QuickStartTest.DeleteBucket(BucketName);
        }

        public string BucketName { get; private set; }
    }

    public class QuickStartTest : BaseTest, IDisposable, IClassFixture<BucketFixture>
    {
        private readonly string _bucketName;
        /// <summary>
        /// Maintain a list of objects that must be deleted at the end of the test.
        /// </summary>
        private readonly SortedDictionary<string, SortedSet<string>> _garbage =
            new SortedDictionary<string, SortedSet<string>>();

        public QuickStartTest(BucketFixture fixture)
        {
            _bucketName = fixture.BucketName;
        }

        /// <summary>
        /// Add an object to delete at the end of the test.
        /// </summary>
        /// <returns>The objectName.</returns>
        private string Collect(string bucketName, string objectName)
        {
            SortedSet<string> objectNames;
            if (!_garbage.TryGetValue(bucketName, out objectNames))
            {
                objectNames = _garbage[bucketName] = new SortedSet<string>();
            }
            objectNames.Add(objectName);
            return objectName;
        }

        /// <summary>
        /// Add an object to delete at the end of the test.
        /// </summary>
        /// <returns>The objectName.</returns>
        private string Collect(string objectName) => Collect(_bucketName, objectName);

        public static void DeleteBucket(string bucketName)
        {
            Eventually(() => AssertSucceeded(Run("delete", bucketName)));
        }

        public void Dispose()
        {
            DeleteGarbage();
        }

        private void DeleteGarbage()
        {
            foreach (var bucket in _garbage)
            {
                List<string> args = new List<string>();
                args.Add("delete");
                args.Add(bucket.Key);
                args.AddRange(bucket.Value);
                AssertSucceeded(Run(args.ToArray()));
            }
            _garbage.Clear();
        }

        /// <summary>
        /// Retry action.
        /// Datastore guarantees only eventual consistency.  Many tests write
        /// an entity and then query it afterward, but may not find it immediately.
        /// </summary>
        /// <param name="action"></param>
        private static void Eventually(Action action)
        {
            int delayMs = 1000;
            for (int i = 0; ; ++i)
            {
                try
                {
                    action();
                    return;
                }
                catch (Xunit.Sdk.XunitException) when (i < 4)
                {
                    Thread.Sleep(delayMs);
                    delayMs *= 2;
                }
            }
        }

        public static string CreateRandomBucket()
        {
            var created = Run("create");
            AssertSucceeded(created);
            var created_regex = new Regex(@"Created\s+(.+)\.\s*", RegexOptions.IgnoreCase);
            var match = created_regex.Match(created.Stdout);
            Assert.True(match.Success);
            string bucketName = match.Groups[1].Value;
            return bucketName;
        }

        [Fact]
        public void TestCreateBucket()
        {
            // Try creating another bucket with the same name.  Should fail.
            var created_again = Run("create", _bucketName);
            Assert.Equal(409, created_again.ExitCode);

            // Try listing the buckets.  We should find the new one.
            Eventually(() =>
            {
                var listed = Run("list");
                AssertSucceeded(listed);
                Assert.Contains(_bucketName, listed.Stdout);
            });
        }

        [Fact]
        public void TestListObjectsInBucket()
        {
            // Try listing the files.  There should be none.
            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Equal("", listed.Stdout);
            });

            var uploaded = Run("upload", _bucketName, Collect("Hello.txt"));
            AssertSucceeded(uploaded);

            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Contains("Hello.txt", listed.Stdout);
            });
        }

        public string[] SplitOutput(string stdout) =>
            stdout.Split('\n')
                .Select((s) => s.Trim()).Where((s) => !string.IsNullOrEmpty(s))
                .OrderBy((s) => s).ToArray();

        [Fact]
        public void TestListObjectsInBucketWithPrefix()
        {
            // Try listing the files.  There should be none.
            Eventually(() =>
            {
                var listed = Run("list", _bucketName);
                AssertSucceeded(listed);
                Assert.Equal("", listed.Stdout);
            });

            // Upload 4 files.
            var uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/1.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/2.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("b/2.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("a/b/3.txt"));
            AssertSucceeded(uploaded);

            Eventually(() =>
            {
                // With no delimiter, we should get all 3 files.
                var listed = Run("list", _bucketName, "a/", null);
                AssertSucceeded(listed);
                Assert.Equal(new string[] {
                    "a/1.txt",
                    "a/2.txt",
                    "a/b/3.txt"
                }, SplitOutput(listed.Stdout));

                // With a delimeter, we should see only direct contents.
                listed = Run("list", _bucketName, "a/", "/");
                AssertSucceeded(listed);
                Assert.Equal(new string[] {
                    "a/1.txt",
                    "a/2.txt",
                }, SplitOutput(listed.Stdout));
            });
        }

        [Fact]
        public void TestDownloadObject()
        {
            var uploaded = Run("upload", _bucketName, Collect("Hello.txt"));
            AssertSucceeded(uploaded);
            uploaded = Run("upload", _bucketName, "Hello.txt", Collect("Hello2.txt"));
            AssertSucceeded(uploaded);

            var downloaded = Run("download", _bucketName, "Hello2.txt");
            AssertSucceeded(downloaded);
            try
            {
                Assert.Equal(File.ReadAllText("Hello.txt"),
                    File.ReadAllText("Hello2.txt"));
                downloaded = Run("download", _bucketName, "Hello.txt",
                    "Hello2.txt");
                AssertSucceeded(downloaded);
                Assert.Equal(File.ReadAllText("Hello.txt"),
                    File.ReadAllText("Hello2.txt"));
            }
            finally
            {
                File.Delete("Hello2.txt");
            }
        }

        [Fact]
        public void TestGetMetadata()
        {
            var uploaded = Run("upload", _bucketName, Collect("Hello.txt"));
            var got = Run("get-metadata", _bucketName, "Hello.txt");
            AssertSucceeded(got);
            Assert.Contains("Generation", got.Stdout);
            Assert.Contains("Size", got.Stdout);
        }

        [Fact]
        public void TestMakePublic()
        {
            var uploaded = Run("upload", _bucketName, Collect("Hello.txt"));
            var got = Run("get-metadata", _bucketName, "Hello.txt");
            AssertSucceeded(got);
            var medialink_regex = new Regex(@"MediaLink:\s?(.+)");
            var match = medialink_regex.Match(got.Stdout);
            Assert.True(match.Success);

            // Before making the file public, fetching the medialink should
            // throw an exception.
            string medialink = match.Groups[1].Value.Trim();
            WebClient webClient = new WebClient();
            Assert.Throws<WebException>(() =>
                webClient.DownloadString(medialink));

            // Make it public and try fetching again.
            var madePublic = Run("make-public", _bucketName, "Hello.txt");
            AssertSucceeded(madePublic);
            var text = webClient.DownloadString(medialink);
            Assert.Equal(File.ReadAllText("Hello.txt"), text);
        }

        [Fact]
        public void TestMove()
        {
            Run("upload", _bucketName, Collect("Hello.txt"));
            // Make sure the file doesn't exist until we move it there.
            var got = Run("get-metadata", _bucketName, "Bye.txt");
            Assert.Equal(404, got.ExitCode);
            // Now move it there.
            AssertSucceeded(Run("move", _bucketName, "Hello.txt", Collect("Bye.txt")));
            // If we try to clean up "Hello.txt", it will fail because it moved.
            _garbage[_bucketName].Remove("Hello.txt");
            AssertSucceeded(Run("get-metadata", _bucketName, "Bye.txt"));
        }

        [Fact]
        public void TestCopy()
        {
            Run("upload", _bucketName, Collect("Hello.txt"));
            using (var otherBucket = new BucketFixture())
            {
                try
                {
                    AssertSucceeded(Run("copy", _bucketName, "Hello.txt",
                        otherBucket.BucketName, "Bye.txt"));
                    Collect(otherBucket.BucketName, "Bye.txt");
                    AssertSucceeded(Run("get-metadata", otherBucket.BucketName,
                        "Bye.txt"));
                }
                finally
                {
                    // Must delete garbage before trying to dispose otherBucket.
                    DeleteGarbage();
                }
            }
        }
    }
}