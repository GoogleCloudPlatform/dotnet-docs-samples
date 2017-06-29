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

    public class AnalyzeTests
    {
        readonly CommandLineRunner _analyze = new CommandLineRunner()
        {
            VoidMain = Analyzer.Main,
            Command = "Analyze"
        };

        [Fact]
        void TestAnalyzeNoArgsSucceeds()
        {
            ConsoleOutput output = _analyze.Run();
            Assert.Equal(0, output.ExitCode);
        }

        [Fact]
        void TestShots()
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
                @"gs://demomaker/cat.mp4");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Cat", output.Stdout);
        }

        // TODO: Make a [FACT] when faces feature is publicly available.
        void TestFacesNotWhitelisted()
        {
            // Analyzing faces is a feature that requires your
            // project id appear on a whitelist.
            Assert.Throws<Grpc.Core.RpcException>(() =>
                _analyze.Run("faces",
                "gs://cloudmleap/video/next/fox-snatched.mp4"));
        }
    }
}
