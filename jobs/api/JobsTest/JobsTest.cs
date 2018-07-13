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

using System.IO;
using Xunit;
using System;
using System.Linq;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Runs the QuickStart console app and test output.
    /// </summary>
    public class QuickStartTests
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
            Assert.Contains("Request Id", output.Stdout);
        }
    }
}
