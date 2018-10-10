// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using GoogleCloudSamples;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DlpTest
{
    public class InspectTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _testSettings;

        public InspectTests(DlpTestFixture fixture)
        {
            _testSettings = fixture;
        }

        [Fact]
        public void TestInspectString()
        {
            // inspect a string with results
            ConsoleOutput outputA = _testSettings.CommandLineRunner.Run(
                "inspectString",
                _testSettings.ProjectId,
                "The name Robert is very common.",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect a string with no results
            ConsoleOutput outputB = _testSettings.CommandLineRunner.Run(
                "inspectString",
                _testSettings.ProjectId,
                "She sells sea shells by the sea shore.",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("No findings", outputB.Stdout);

            // inspect a string with custom info types
            ConsoleOutput outputC = _testSettings.CommandLineRunner.Run(
                "inspectString",
                _testSettings.ProjectId,
                "My name is Robert and my phone number is (425) 634-9233.",
                "-c", "Robert",
                "-r", "\\(\\d{3}\\) \\d{3}-\\d{4}"
            );
            Assert.Contains("CUSTOM_DICTIONARY", outputC.Stdout);
            Assert.Contains("CUSTOM_REGEX_0", outputC.Stdout);

            // inspect a string with no results
            ConsoleOutput outputD = _testSettings.CommandLineRunner.Run(
                "inspectString",
                _testSettings.ProjectId,
                "She sells sea shells by the sea shore.",
                "-c", "Robert",
                "-r", "\\(\\d{3}\\) \\d{3}-\\d{4}"
            );
            Assert.Contains("No findings", outputD.Stdout);
        }

        [Fact]
        public void TestInspectFile()
        {
            // inspect a text file with results
            ConsoleOutput outputA = _testSettings.CommandLineRunner.Run(
                "inspectFile",
                _testSettings.ProjectId,
                _testSettings.ResourcePath + "test.txt",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect an image file with results
            ConsoleOutput outputB = _testSettings.CommandLineRunner.Run(
                "inspectFile",
                _testSettings.ProjectId,
                _testSettings.ResourcePath + "test.png",
                "-i", "PHONE_NUMBER,EMAIL_ADDRESS"
            );
            Assert.Contains("PHONE_NUMBER", outputB.Stdout);

            // inspect a file with no results
            ConsoleOutput outputC = _testSettings.CommandLineRunner.Run(
                "inspectFile",
                _testSettings.ProjectId,
                _testSettings.ResourcePath + "harmless.txt"
            );
            Assert.Contains("No findings", outputC.Stdout);

            // inspect a text file with custom info types
            ConsoleOutput outputD = _testSettings.CommandLineRunner.Run(
                "inspectFile",
                _testSettings.ProjectId,
                _testSettings.ResourcePath + "test.txt",
                "-c", "Robert",
                "-r", "Frost"
            );
            Assert.Contains("CUSTOM_DICTIONARY", outputD.Stdout);
            Assert.Contains("CUSTOM_REGEX_0", outputD.Stdout);
        }

        /*[Fact]
        public void TestInspectDatastore()
        {
            ConsoleOutput output = _dlp.Run('inspectDatastore',
                ProjectId,
                'kind' => 'Person',
                'topic-id' => getenv('DLP_TOPIC'),
                'subscription-id' => getenv('DLP_SUBSCRIPTION'),
                'namespace' => 'DLP'
            ]);
            Assert.Contains("PERSON_NAME", output.Stdout);
        }

        [Fact]
        public void TestInspectBigquery()
        {
            ConsoleOutput output = _dlp.Run('inspectBigquery',
                ProjectId,
                ProjectId,
                'dataset' => 'integration_tests_dlp',
                'table' => 'harmful',
                'topic-id' => getenv('DLP_TOPIC'),
                'subscription-id' => getenv('DLP_SUBSCRIPTION')
            ]);
            Assert.Contains("PERSON_NAME", output.Stdout);
        }

        [Fact]
        public void TestInspectGCS()
        {
            ConsoleOutput output = _dlp.Run('inspectGcs',
                ProjectId,
                'bucket-id' => getenv('DLP_BUCKET'),
                'file' => 'harmful.csv',
                'topic-id' => getenv('DLP_TOPIC'),
                'subscription-id' => getenv('DLP_SUBSCRIPTION')
            ]);
            Assert.Contains("PERSON_NAME", output.Stdout);
        }*/
    }
}
