/*
 * Copyright (c) 2016 Google Inc.
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
    public class AnalyzeTest
    {
        readonly CommandLineRunner _analyze = new CommandLineRunner()
        {
            VoidMain = Analyze.Main,
            Command = "dotnet run"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _analyze.Run(args);
        }

        private static readonly string s_text =
            "Santa Claus Conquers the Martians is a terrible movie. "
            + "It's so bad, it's good. This is a classic example.";

        private static readonly string s_gcsUri =
            "gs://silver-python2-testing/SantaClausText.txt";

        [Fact]
        public void CommandLinePrintsUsageTest()
        {
            var output = Run();
            var entitiesOutput = Run("entities");
            var badCommandOutput = Run("badcommand", "text");
            Assert.Equal(Analyze.Usage, output.Stdout);
            Assert.Equal(Analyze.Usage, entitiesOutput.Stdout);
            Assert.Equal(Analyze.Usage, badCommandOutput.Stdout);
        }

        [Fact]
        public void EntitiesTest()
        {
            var output = Run("entities", s_text);
            Assert.Contains("Entities:", output.Stdout);
            Assert.Contains("Name: Santa Claus Conquers the Martians", output.Stdout);
        }

        [Fact]
        public void EntitiesFromFileTest()
        {
            var output = Run("entities", s_gcsUri);
            Assert.Contains("Entities:", output.Stdout);
            Assert.Contains("Name: Santa Claus Conquers the Martians", output.Stdout);
        }

        [Fact]
        public void SyntaxTest()
        {
            var output = Run("syntax", s_text);
            Assert.Contains("Sentences:", output.Stdout);
            Assert.Contains("0: Santa Claus Conquers the Martians is a terrible movie.", output.Stdout);
            Assert.Contains("55: It's so bad, it's good.", output.Stdout);
            Assert.Contains("Tokens:", output.Stdout);
            Assert.Contains("Noun Santa", output.Stdout);
            Assert.Contains("Verb Conquers", output.Stdout);
        }

        [Fact]
        public void SyntaxFromFileTest()
        {
            var output = Run("syntax", s_gcsUri);
            Assert.Contains("Sentences:", output.Stdout);
            Assert.Contains("0: Santa Claus Conquers the Martians is a terrible movie.", output.Stdout);
            Assert.Contains("55: It's so bad, it's good.", output.Stdout);
        }

        [Fact]
        public void SentimentTest()
        {
            var output = Run("sentiment", s_text);
            Assert.Contains("Score: ", output.Stdout);
            Assert.Contains("Magnitude: ", output.Stdout);
            Assert.Contains("Sentence level sentiment:", output.Stdout);
            var regex = new System.Text.RegularExpressions.Regex(
                @"a terrible movie.:\s*\((.+)\)");
            var match = regex.Match(output.Stdout);
            Assert.True(match.Success);
            double score = double.Parse(match.Groups[1].Value);
            Assert.True(score < 0);
        }

        [Fact]
        public void SentimentFromFileTest()
        {
            var output = Run("sentiment", s_gcsUri);
            Assert.Contains("Score: ", output.Stdout);
            Assert.Contains("Magnitude: ", output.Stdout);
        }

        [Fact]
        public void EntitySentimentTest()
        {
            var output = Run("entity-sentiment", s_text);
            Assert.Contains("Entity Sentiment:", output.Stdout);
            Assert.Contains("Santa Claus Conquers the Martians (31%)", output.Stdout);
        }

        [Fact]
        public void EntitySentimentFromFileTest()
        {
            var output = Run("entity-sentiment", s_gcsUri);
            Assert.Contains("Entity Sentiment:", output.Stdout);
            Assert.Contains("Santa Claus Conquers the Martians (31%)", output.Stdout);
        }

        [Fact]
        public void ClassifyTextTest()
        {
            var output = Run("classify-text", s_text);
            Assert.Contains("Categories:", output.Stdout);
            Assert.Contains("Category: /Arts & Entertainment", output.Stdout);
        }
        
        [Fact]
        public void ClassifyTextFromFileTest()
        {
            var output = Run("classify-text", s_gcsUri);
            Assert.Contains("Categories:", output.Stdout);
            Assert.Contains("Category: /Arts & Entertainment", output.Stdout);
        }

        [Fact]
        public void EverythingTest()
        {
            var output = Run("everything", s_text);
            Assert.Contains("Language: en", output.Stdout);
            Assert.Contains("Score: ", output.Stdout);
            Assert.Contains("Magnitude: ", output.Stdout);
            Assert.Contains("Sentences:", output.Stdout);
            Assert.Contains("0: Santa Claus Conquers the Martians is a terrible movie.", output.Stdout);
            Assert.Contains("55: It's so bad, it's good.", output.Stdout);
            Assert.Contains("Entities:", output.Stdout);
            Assert.Contains("Name: Santa Claus Conquers the Martians", output.Stdout);
            Assert.Contains("Category: /Arts & Entertainment", output.Stdout);
        }
    }

    /// <summary>
    /// Runs the QuickStart console app and test output.
    /// </summary>
    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRun()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Magnitude", output.Stdout);
        }
    }
}