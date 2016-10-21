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
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class QuickStartTest
    {
        private struct RunResult
        {
            public int ExitCode;
            public string Stdout;
        };

        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        private RunResult Run(params string[] arguments)
        {
            Console.Write("QuickStart.exe ");
            Console.WriteLine(string.Join(" ", arguments));
            var standardOut = Console.Out;
            using (var output = new StringWriter())
            {
                Console.SetOut(output);
                try
                {
                    return new RunResult()
                    {
                        ExitCode = QuickStart.Main(arguments),
                        Stdout = output.ToString()
                    };
                }
                finally
                {
                    Console.SetOut(standardOut);
                }
            }
        }

        [Fact]
        public void TestNoArgs()
        {
            // Create a randomly named bucket.
            var ran = Run();
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }

        [Fact]
        public void TestBadCommand()
        {
            // Create a randomly named bucket.
            var ran = Run("throb");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }

        [Fact]
        public void TestMissingDeleteArg()
        {
            // Create a randomly named bucket.
            var ran = Run("delete");
            Assert.Equal(-1, ran.ExitCode);
            Assert.Contains("QuickStart", ran.Stdout);
        }

        [Fact]
        public void TestCreateAndDelete()
        {
            // Create a randomly named bucket.
            var created = Run("create");
            Assert.Equal(0, created.ExitCode);
            var created_regex = new Regex(@"Created\s+(.+)\.\s*", RegexOptions.IgnoreCase);
            var match = created_regex.Match(created.Stdout);
            Assert.True(match.Success);
            string bucketName = match.Groups[1].Value;
            RunResult deleted;
            try
            {
                // Try creating another bucket with the same name.  Should fail.
                var created_again = Run("create", bucketName);
                Assert.Equal(409, created_again.ExitCode);

                // Try listing the buckets.  We should find the new one.
                var listed = Run("list");
                Assert.Equal(0, listed.ExitCode);
                Assert.Contains(bucketName, listed.Stdout);
            }
            finally
            {
                deleted = Run("delete", bucketName);
            }
            Assert.Equal(0, deleted.ExitCode);
            // Make sure a second attempt to delete fails.
            Assert.Equal(404, Run("delete", bucketName).ExitCode);
        }

        [Fact]
        public void TestListObjectsInBucket()
        {
            var created = Run("create");
            Assert.Equal(0, created.ExitCode);
            var created_regex = new Regex(@"Created\s+(.+)\.\s*", RegexOptions.IgnoreCase);
            var match = created_regex.Match(created.Stdout);
            Assert.True(match.Success);
            string bucketName = match.Groups[1].Value;
            try
            {
                // Try listing the files.  There should be none.
                var listed = Run("list", bucketName);
                Assert.Equal(0, listed.ExitCode);
                Assert.Equal("", listed.Stdout);

                var uploaded = Run("upload", bucketName, "Hello.txt");
                Assert.Equal(0, uploaded.ExitCode);
                var deleted = Run("delete", bucketName, "Hello.txt");
                Assert.Equal(0, deleted.ExitCode);
            }
            finally
            {
                Assert.Equal(0, Run("delete", bucketName).ExitCode);
            }
        }
    }
}