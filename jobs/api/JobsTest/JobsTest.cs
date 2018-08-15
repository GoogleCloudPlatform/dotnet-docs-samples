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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace GoogleCloudSamples
{

    public class LaunchSettingsFixture : IDisposable
    {
        public LaunchSettingsFixture()
        {
            using (var file = File.OpenText("Properties\\launchSettings.json"))
            {
                JsonTextReader reader = new JsonTextReader(file);
                JObject jObject = JObject.Load(reader);

                List<JProperty> variables = jObject
                    .GetValue("profiles")
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Runs the QuickStart console app and test output.
    /// </summary>
    public class QuickStartTests : IClassFixture<LaunchSettingsFixture>
    {
        readonly LaunchSettingsFixture _fixture;
        public readonly ITestOutputHelper _testOutput;

        public QuickStartTests(LaunchSettingsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _testOutput = output;
        }

        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "QuickStart"
        };

        readonly CommandLineRunner _quickStartCompanies = new CommandLineRunner()
        {
            VoidMain = QuickStartCompanies.Main,
            Command = "QuickStartCompanies"
        };

        readonly CommandLineRunner _quickStartJobs = new CommandLineRunner()
        {
            VoidMain = QuickStartJobs.Main,
            Command = "QuickStartJobs"
        };

        readonly CommandLineRunner _quickStartSearch = new CommandLineRunner()
        {
            VoidMain = QuickStartSearch.Main,
            Command = "QuickStartSearch"
        };

        [Fact]
        public void TestRun()
        {
            ConsoleOutput output = _quickStart.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Request Id", output.Stdout);
        }

        [Fact]
        public void TestCompanies()
        {
            ConsoleOutput output = _quickStartCompanies.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Created", output.Stdout);
            Assert.Contains("existed", output.Stdout);
            Assert.Contains("Deleted", output.Stdout);
        }

        [Fact]
        public void TestJobs()
        {
            ConsoleOutput output = _quickStartJobs.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Job created", output.Stdout);
            Assert.Contains("Custom attribute", output.Stdout);
            Assert.Contains("Job exists", output.Stdout);
            Assert.Contains("Job deleted", output.Stdout);
        }

        [Fact]
        public void TestSearch()
        {
            var output = _quickStartSearch.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Jobs searched", output.Stdout);
            Assert.Contains("Histogram search", output.Stdout);
            Assert.Contains("Completion results", output.Stdout);
        }
    }
}
