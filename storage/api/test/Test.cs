/*
 * Copyright (c) 2015 Google Inc.
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
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class StorageSampleTest
    {
        /// <summary>Runs StorageSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public string Run(params string[] arguments)
        {
            var standardOut = Console.Out;

            using (var output = new StringWriter())
            {
                Console.SetOut(output);

                try
                {
                    StorageProgram.Main(arguments);
                    return output.ToString();
                }
                finally
                {
                    Console.SetOut(standardOut);
                }
            }
        }

        [Fact]
        public void CommandLinePrintsUsageTest()
        {
            Assert.Contains(
                "Usage: StorageSample.exe [command]",
                Run()
            );
        }

        [Fact]
        public void ListBucketsTest()
        {
            var bucketName = Environment.GetEnvironmentVariable("GOOGLE_BUCKET");

            Assert.Contains(
                $"Bucket: {bucketName}",
                Run("ListBuckets")
            );
        }

        [Fact]
        public void ListObjectsTest()
        {
            Run("UploadStream");

            Assert.Contains(
                "Object: my-file.txt",
                Run("ListObjects")
            );
        }

        [Fact]
        public void UploadAndDownloadStreamTest()
        {
            if (Run("ListObjects").Contains("Object: my-file.txt"))
            {
                Run("DeleteObject");
            }

            Assert.Contains(
                "Uploaded my-file.txt",
                Run("UploadStream")
            );

            Assert.Contains(
                "Downloaded my-file.txt with content: My text object content",
                Run("DownloadStream")
            );
        }

        [Fact]
        public void DeleteObjectTest()
        {
            Run("UploadStream");

            Assert.Contains(
                "Object: my-file.txt",
                Run("ListObjects")
            );

            Assert.Contains(
                "Deleted my-file.txt",
                Run("DeleteObject")
            );

            Assert.DoesNotContain(
                "Object: my-file.txt",
                Run("ListObjects")
            );
        }

        [Fact]
        public void DownloadToFileTest()
        {
            Run("UploadStream");

            if (File.Exists("downloaded-file.txt"))
            {
                File.Delete("downloaded-file.txt");
            }

            Assert.False(File.Exists("downloaded-file.txt"));

            Assert.Contains(
                "Downloaded my-file.txt to downloaded-file.txt",
                Run("DownloadToFile")
            );

            Assert.True(File.Exists("downloaded-file.txt"));

            Assert.Equal(
                "My text object content",
                File.ReadAllText("downloaded-file.txt")
            );
        }
    }
}