/*
 * Copyright (c) 2017 Google Inc.
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
    public class QuickStartTest
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "QuickStart"
        };

        [Fact]
        public void TestRun()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }
    }

    public abstract class CommonRecognizeTests
    {
        protected readonly CommandLineRunner _recognize = new CommandLineRunner()
        {
            Main = Recognize.Main,
            Command = "Recognize"
        };
        /// <summary>
        /// Derived classes implement this function to examine the file
        /// locally, or first upload it to Google Cloud Storage and then
        /// examine it.
        /// </summary>
        /// <param name="args">Command line arguments to Main().</param>
        protected abstract ConsoleOutput Run(params string[] args);

        protected string _audioRawPath = Path.Combine("resources", "audio.raw");
        protected string _audioFlacPath = Path.Combine("resources", "audio.flac");
        protected string _audioWavPath = Path.Combine("resources", "commercial_mono.wav");

        [Fact]
        public void TestSync()
        {
            var output = Run("sync", _audioRawPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }
    }

    public class LocalRecognizeTests : CommonRecognizeTests
    {
        protected override ConsoleOutput Run(params string[] args) =>
            _recognize.Run(args);

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/723")]
        public void TestStreaming()
        {
            var output = _recognize.Run("stream", _audioRawPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }

        [Fact(Skip = "Unreliable on automated test machines.")]
        public void TestListen()
        {
            var output = _recognize.Run("listen", "3");
            if (0 == output.ExitCode)
                Assert.Contains("Speak now.", output.Stdout);
            else
                Assert.Contains("No microphone.", output.Stdout);
        }

        [Fact]
        public void TestFlac()
        {
            var output = _recognize.Run("rec", "-e", "Flac", _audioFlacPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }

        [Fact]
        public void TestSyncWithCredentials()
        {
            var output = Run("sync-creds", _audioRawPath,
                System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }

        [Fact]
        public void TestWithContext()
        {
            string stdin = "Good day!\nBye bye.\n\n";
            var output = _recognize.RunWithStdIn(stdin, "with-context", _audioRawPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
        }

        [Fact]
        public void TestSyncWords()
        {
            var output = Run("sync", "-w", _audioRawPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
            Assert.Contains("WordStartTime:", output.Stdout);
        }

        [Fact]
        public void TestSyncEnhancedModel()
        {
            var output = Run("sync", "-e", _audioWavPath);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("regular Chromecast is fine", output.Stdout);
        }
    }

    public class CloudStorageRecognizeTests : CommonRecognizeTests, IClassFixture<RandomBucketFixture>, System.IDisposable
    {
        readonly string _bucketName;
        readonly BucketCollector _bucketCollector;

        public CloudStorageRecognizeTests(RandomBucketFixture bucketFixture)
        {
            _bucketName = bucketFixture.BucketName;
            _bucketCollector = new BucketCollector(_bucketName);
        }

        string Upload(string localPath)
        {
            string objectName = Path.GetFileName(localPath);
            string gsPath = $"gs://{_bucketName}/{objectName}";
            _bucketCollector.CopyToBucket(localPath, objectName);
            return gsPath;
        }

        [Fact]
        public void TestAsync()
        {
            var output = Run("async", Upload(_audioRawPath));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
            Assert.Contains("how", output.Stdout);
        }

        [Fact]
        public void TestAsyncWords()
        {
            var output = Run("async", "-w", Upload(_audioRawPath));
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Brooklyn", output.Stdout);
            Assert.Contains("WordStartTime:", output.Stdout);
        }

        public void Dispose()
        {
            ((IDisposable)_bucketCollector).Dispose();
        }

        protected override ConsoleOutput Run(params string[] args)
        {
            return _recognize.Run(args);
        }
    }
}
