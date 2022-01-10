// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using grs_search.search;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace grs_search.Tests
{
    [TestClass]
    public class SearchWithFacetSpecTest
    {
        private const string SearchFolderName = "grs-search";

        private const string WindowsTerminalVarName = "ComSpec";
        private const string UnixTerminalVarName = "SHELL";
        private const string WindowsTerminalPrefix = "/c ";
        private const string UnixTerminalPrefix = "-c ";

        private static readonly string WorkingDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, SearchFolderName);

        private static readonly string CurrentOperatingSystemName = Environment.OSVersion.VersionString;
        private static readonly string CurrentTerminalVarName = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalVarName : UnixTerminalVarName;
        private static readonly string CurrentTerminalPrefix = CurrentOperatingSystemName.Contains("Windows") ? WindowsTerminalPrefix : UnixTerminalPrefix;
        private static readonly string CurrentTerminalFile = "/bin/bash";

        private static readonly string CommandLineArguments = "-c \"dotnet run -- SearchWithFacetSpec\"";

        [TestMethod]
        public void TestSearchWithFacetSpec()
        {
            const string ExpectedProductTitle = "Tee";
            const string ExpectedFacetKey = "colorFamilies";

            var response = SearchWithFacetSpec.Search();

            var actualProductTitle = response.ToArray()[0].Results[0].Product.Title;
            var actualFacetKey = response.ToArray()[0].Facets[0].Key;

            Assert.IsTrue(actualProductTitle.Contains(ExpectedProductTitle));
            Assert.AreEqual(actualFacetKey, ExpectedFacetKey);
        }

        [TestMethod]
        public void TestOutputSearchWithFacetSpec()
        {
            string consoleOutput = string.Empty;

            var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, CommandLineArguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = WorkingDirectory
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();

                consoleOutput = process.StandardOutput.ReadToEnd();
            }

            Assert.IsTrue(consoleOutput.Contains("Search. request:"));
            Assert.IsTrue(consoleOutput.Contains("Search. response:"));

            // Check the response contains some products
            Assert.IsTrue(consoleOutput.Contains("\"id\":"));
            Assert.IsTrue(consoleOutput.Contains("\"product\":"));
            Assert.IsTrue(Regex.Match(consoleOutput, "(.*)\"facets\":(.*)\"colorFamilies\"(.*)", RegexOptions.Singleline).Success);
        }
    }
}