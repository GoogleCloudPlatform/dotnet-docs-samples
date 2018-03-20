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
        private readonly DlpTestFixture testSettings;

        public InspectTests(DlpTestFixture fixture)
        {
            testSettings = fixture;
        }

        [Fact]
        public void TestInspectString()
        {
            // inspect a string with results
            ConsoleOutput outputA = testSettings.CommandLineRunner.Run(
                "inspectString",
                testSettings.ProjectId,
                "The name Robert is very common.",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect a string with no results
            ConsoleOutput outputB = testSettings.CommandLineRunner.Run(
                "inspectString",
                testSettings.ProjectId,
                "She sells sea shells by the sea shore."
            );
            Assert.Contains("No findings", outputB.Stdout);
        }

        [Fact]
        public void TestInspectFile()
        {
            // inspect a text file with results
            ConsoleOutput outputA = testSettings.CommandLineRunner.Run(
                "inspectFile",
                testSettings.ProjectId,
                testSettings.ResourcePath + "test.txt",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect an image file with results
            ConsoleOutput outputB = testSettings.CommandLineRunner.Run(
                "inspectFile",
                testSettings.ProjectId,
                testSettings.ResourcePath + "test.png",
                "-i", "PHONE_NUMBER,EMAIL_ADDRESS"
            );
            Assert.Contains("PHONE_NUMBER", outputB.Stdout);

            // inspect a file with no results
            ConsoleOutput outputC = testSettings.CommandLineRunner.Run(
                "inspectFile",
                testSettings.ProjectId,
                testSettings.ResourcePath + "harmless.txt"
            );
            Assert.Contains("No findings", outputC.Stdout);
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
