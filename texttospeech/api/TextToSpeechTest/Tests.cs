// Copyright(c) 2018 Google Inc.
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
//

using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace GoogleCloudSamples
{
    public class TextToSpeechTests
    {
        readonly CommandLineRunner _runner = new CommandLineRunner()
        {
            Command = "TextToSpeech",
            VoidMain = TextToSpeech.Main
        };

        [Fact]
        public void TestList()
        {
            ConsoleOutput output = _runner.Run("list");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("en-US-Standard-B", output.Stdout);
        }

        [Fact]
        public void TestSynthesizeText()
        {
            ConsoleOutput output = _runner.Run("synthesize", "Hello World");
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("output.mp3"));

            FileInfo fileInfo = new FileInfo("output.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }

        [Fact]
        public void TestSynthesizeSSML()
        {
            string ssml = "<speak>This is " +
                "<say-as interpret-as='characters'>SSML</say-as></speak>";

            ConsoleOutput output = _runner.Run("synthesize", ssml, "--f", "ssml");
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("output.mp3"));

            FileInfo fileInfo = new FileInfo("output.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }

        [Fact]
        public void TestSynthesizeTextFile()
        {
            string filePath = Path.Combine("data", "hello.txt");
            ConsoleOutput output = _runner.Run("synthesize-file", filePath);
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("output.mp3"));

            FileInfo fileInfo = new FileInfo("output.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }

        [Fact]
        public void TestSynthesizeSSMLFile()
        {
            string filePath = Path.Combine("data", "hello.ssml");
            ConsoleOutput output = _runner.Run("synthesize-file", filePath, "--f", "ssml");
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("output.mp3"));

            FileInfo fileInfo = new FileInfo("output.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }

        [Fact]
        public void TestSynthesizeTextWithEffectsProfile()
        {
            ConsoleOutput output = _runner.Run("synthesize-with-profile",
                                               "Hello World",
                                               "-o",
                                               "output.mp3",
                                               "-e",
                                               "headphone-class-device");
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("output.mp3"));

            FileInfo fileInfo = new FileInfo("output.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }
    }

    public class QuickStartTests
    {
        readonly CommandLineRunner _runner = new CommandLineRunner()
        {
            Command = "QuickStart ",
            VoidMain = QuickStart.Main
        };

        [Fact]
        public void TestQuickStart()
        {
            ConsoleOutput output = _runner.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.True(File.Exists("sample.mp3"));

            FileInfo fileInfo = new FileInfo("sample.mp3");
            Assert.True(fileInfo.Length > 0);

            // Clean up
            File.Delete("output.mp3");
        }
    }
}
