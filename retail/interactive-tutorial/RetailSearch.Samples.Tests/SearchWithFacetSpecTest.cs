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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace RetailSearch.Samples.Tests
{
    public class SearchWithFacetSpecTest
    {
        private const string SearchFolderName = "RetailSearch.Samples";

        private const string DotNetCommand = "dotnet run -- SearchWithFacetSpecTutorial";

        private const string WindowsTerminalName = "cmd.exe";
        private const string UnixTerminalName = "/bin/bash";
        private const string WindowsTerminalPrefix = "/c ";
        private const string UnixTerminalPrefix = "-c ";
        private const string WindowsTerminalQuotes = "";
        private const string UnixTerminalQuotes = "\"";

        private static readonly string WorkingDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, SearchFolderName);

        private static readonly bool CurrentOSIsWindows = Environment.OSVersion.VersionString.Contains("Windows");
        private static readonly string CurrentTerminalPrefix = CurrentOSIsWindows ? WindowsTerminalPrefix : UnixTerminalPrefix;
        private static readonly string CurrentTerminalFile = CurrentOSIsWindows ? WindowsTerminalName : UnixTerminalName;
        private static readonly string CurrentTerminalQuotes = CurrentOSIsWindows ? WindowsTerminalQuotes : UnixTerminalQuotes;

        private static readonly string CommandLineArguments = CurrentTerminalPrefix + CurrentTerminalQuotes + DotNetCommand + CurrentTerminalQuotes;

        [Fact]
        public void TestSearchWithFacetSpec()
        {
            const string ExpectedProductTitle = "Tee";
            const string ExpectedFacetKey = "colorFamilies";

            var response = SearchWithFacetSpecTutorial.Search();

            var actualProductTitle = response.ToArray()[0].Results[0].Product.Title;
            var actualFacetKey = response.ToArray()[0].Facets[0].Key;

            Assert.Contains(ExpectedProductTitle, actualProductTitle);
            Assert.Equal(ExpectedFacetKey, actualFacetKey);
        }
         
        [Fact]
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

            Assert.Contains("Search. request:", consoleOutput);
            Assert.Contains("Search. response:", consoleOutput);

            // Check the response contains some products:
            Assert.Contains("\"id\":", consoleOutput);
            Assert.Contains("\"product\":", consoleOutput);
            Assert.True(Regex.Match(consoleOutput, "(.*)Facets:(.*)\"colorFamilies\"(.*)", RegexOptions.Singleline).Success);
        }
    }
}