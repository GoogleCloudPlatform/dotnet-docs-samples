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

using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class CommandLineUtilTest
    {
        readonly CommandLineRunner _runner = new CommandLineRunner()
        {
            Main = CommandLineUtilSample.Main
        };

        ConsoleOutput Run(params string[] args) => _runner.Run(args);

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        public void TestVerbsThatReturnZero(int verbNumber)
        {
            var output = Run($"v{verbNumber}");
            Assert.Equal(0, output.ExitCode);
        }

        [Theory]
        [InlineData(5)]
        public void TestVerbsThatReturnNonZero(int verbNumber)
        {
            var output = Run($"v{verbNumber}");
            Assert.Equal(5, output.ExitCode);
        }
    }
}
