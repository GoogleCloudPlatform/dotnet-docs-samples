// Copyright(c) 2017 Google Inc.
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

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace GoogleCloudSamples.VideoIntelligence
{
    public class QuickStartTests
    {
        [Fact]
        public void TestQuickStart()
        {
            CommandLineRunner runner = new CommandLineRunner()
            {
                VoidMain = QuickStart.Main,
                Command = "QuickStart"
            };
            var result = runner.Run();
            Assert.Equal(0, result.ExitCode);
        }
    }

    public class AnalyzeTests : IDisposable
    {
        readonly List<string> _tempFiles = new List<string>();

        readonly CommandLineRunner _analyze = new CommandLineRunner()
        {
            VoidMain = Analyzer.Main,
            Command = "Analyze"
        };

        [Fact(Skip = "Triggers infinite loop described here: https://github.com/commandlineparser/commandline/commit/95ded2dbcc5285302723e68221cd30a72444ba84")]
        void TestAnalyzeNoArgsSucceeds()
        {
            ConsoleOutput output = _analyze.Run();
            Assert.Equal(0, output.ExitCode);
        }

        static string SplitGcsUri(string uri, out string bucket)
        {
            string[] chunks = uri.Split(new char[] { '/' }, 4);
            bucket = chunks[2];
            return chunks[3];
        }

        [Fact]
        void TestSplitGcsUri()
        {
            string bucket;
            string objectName = SplitGcsUri("gs://cloudmleap/video/next/fox-snatched.mp4",
                out bucket);
            Assert.Equal("cloudmleap", bucket);
            Assert.Equal("video/next/fox-snatched.mp4", objectName);
        }

        string DownloadGcsObject(string uri)
        {
            var storage = Google.Cloud.Storage.V1.StorageClient.Create();
            string tempFilePath = Path.GetTempFileName();
            using (Stream m = File.OpenWrite(tempFilePath))
            {
                string bucket;
                string objectName = SplitGcsUri(uri, out bucket);
                storage.DownloadObject(bucket, objectName, m);
            }
            _tempFiles.Add(tempFilePath);
            return tempFilePath;
        }

        [Fact]
        void TestShotsGcs()
        {
            ConsoleOutput output = _analyze.Run("shots",
                "gs://cloudmleap/video/next/fox-snatched.mp4");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Start Time Offset", output.Stdout);
            Assert.Contains("End Time Offset", output.Stdout);
        }

        [Fact]
        void TestLabels()
        {
            ConsoleOutput output = _analyze.Run("labels",
                DownloadGcsObject(@"gs://demomaker/cat.mp4"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Cat", output.Stdout, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        void TestLabelsGcs()
        {
            ConsoleOutput output = _analyze.Run("labels",
                @"gs://demomaker/cat.mp4");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Cat", output.Stdout, StringComparison.InvariantCultureIgnoreCase);
        }

        void TestFaces()
        {
            ConsoleOutput output =
                _analyze.Run("faces", DownloadGcsObject("gs://demomaker/gbike.mp4"));
            Assert.Equal(0, output.ExitCode);
        }

        [Fact]
        void TestExplicitContentGcs()
        {
            ConsoleOutput output =
                _analyze.Run("explicit-content", "gs://demomaker/gbike.mp4");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Pornography", output.Stdout);
        }

        void IDisposable.Dispose()
        {
            foreach (string tempFilePath in _tempFiles)
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
