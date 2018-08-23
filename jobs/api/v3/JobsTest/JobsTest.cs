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
    /// <summary>
    /// Runs the QuickStart console app and test output.
    /// </summary>
    public class QuickStartTests
    {
        public readonly ITestOutputHelper _testOutput;

        public QuickStartTests(ITestOutputHelper output)
        {
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

    /// <summary>
    /// Runs the HowTo console app and test output.
    /// </summary>
    public class HowToTests
    {
        public readonly ITestOutputHelper _testOutput;

        public HowToTests(ITestOutputHelper output)
        {
            _testOutput = output;
        }

        readonly CommandLineRunner _howToCompany = new CommandLineRunner()
        {
            VoidMain = Companies.Main,
            Command = "HowToCompanies"
        };

        readonly CommandLineRunner _howToJobs = new CommandLineRunner()
        {
            VoidMain = Jobs.Main,
            Command = "HowToJobs"
        };

        readonly CommandLineRunner _howToBatch = new CommandLineRunner()
        {
            VoidMain = BatchOperations.Main,
            Command = "HowToBatchOperations"
        };

        readonly CommandLineRunner _howToCustomAttributes = new CommandLineRunner()
        {
            VoidMain = CustomAttributes.Main,
            Command = "HowToCustomAttributes"
        };

        readonly CommandLineRunner _howToSearch = new CommandLineRunner()
        {
            VoidMain = Search.Main,
            Command = "HowToCustomAttributes"
        };

        readonly CommandLineRunner _howToEmailAlerts = new CommandLineRunner()
        {
            VoidMain = EmailAlerts.Main,
            Command = "HowToCustomAttributes"
        };

        readonly CommandLineRunner _howToFeaturedJobs = new CommandLineRunner()
        {
            VoidMain = FeaturedJobs.Main,
            Command = "HowToCustomAttributes"
        };

        [Fact]
        public void HowToCompaniesTest()
        {
            var output = _howToCompany.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Company updated", output.Stdout);
            Assert.Contains("Company updated with updateMask", output.Stdout);
        }

        [Fact]
        public void HowToJobsTest()
        {
            var output = _howToJobs.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Job updated", output.Stdout);
            Assert.Contains("Job updated with fieldMask", output.Stdout);
        }

        [Fact]
        public void HowToBatchTest()
        {
            var output = _howToBatch.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Software Engineer", output.Stdout);
            Assert.Contains("Hardware Engineer", output.Stdout);
        }

        [Fact]
        public void HowToCustomAttributesTest()
        {
            var output = _howToCustomAttributes.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Created custom job", output.Stdout);
            Assert.Contains("Searched on custom attribute", output.Stdout);
            Assert.Contains("Searched on custom long attribute", output.Stdout);
            Assert.Contains("Searched on cross-field-filtering", output.Stdout);
        }

        [Fact]
        public void HowToSearchTest()
        {
            var output = _howToSearch.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Jobs category searched", output.Stdout);
            Assert.Contains("Jobs employment type searched", output.Stdout);
            Assert.Contains("Jobs language codes searched", output.Stdout);
            Assert.Contains("Jobs company display names searched", output.Stdout);
            Assert.Contains("Jobs compensation searched", output.Stdout);
            Assert.Contains("Jobs basic location searched", output.Stdout);
            Assert.Contains("Jobs location and keyword searched", output.Stdout);
            Assert.Contains("Jobs city level searched", output.Stdout);
            Assert.Contains("Jobs multi location searched", output.Stdout);
            Assert.Contains("Jobs broadening searched", output.Stdout);
            Assert.Contains("Jobs commute searched", output.Stdout);
        }

        [Fact]
        public void HowToEmailAlertsTest()
        {
            var output = _howToEmailAlerts.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Email alerts searched", output.Stdout);
        }

        [Fact]
        public void HowToFeaturedJobsTest()
        {
            var output = _howToFeaturedJobs.Run();
            _testOutput.WriteLine(output.Stdout);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Featured job generated", output.Stdout);
            Assert.Contains("Featured jobs searched", output.Stdout);
        }
    }
}
