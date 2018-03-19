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

using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudKMS.v1;
using Google.Apis.CloudKMS.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class DlpTestFixture
    {
        public readonly string ProjectId;
        public readonly string WrappedKey;
        public readonly string KeyName;

        public DlpTestFixture()
        {
            // TODO remove
            Environment.SetEnvironmentVariable("GOOGLE_PROJECT_ID", "nodejs-docs-samples");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "/Users/anassri/nodejs-docs-samples.json");
            Environment.SetEnvironmentVariable("DLP_DEID_KEY_NAME", "projects/nodejs-docs-samples/locations/global/keyRings/integration-tests-dlp/cryptoKeys/test-key");
            Environment.SetEnvironmentVariable("DLP_DEID_WRAPPED_KEY", "CiQAaNd+NKZwUklWRkR/57xnFbkQX2YISRHDMpiOG4q92ISwuOkSQQASRgq4htmOs+LXldmKxRvmQ+8MQz3o8xq7zSjG4N0rQbcMgPG7hONPp+PhyKVVbLNds5gMKmx1jclPSTfQT+bH");

            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;


            // Fetch the test key from an environment variable
            KeyName = Environment.GetEnvironmentVariable("DLP_DEID_KEY_NAME");
            WrappedKey = Environment.GetEnvironmentVariable("DLP_DEID_WRAPPED_KEY");
        }
    }

    // TODO reconcile these with the "simple" tests below
    public partial class DlpTest : IClassFixture<DlpTestFixture>
    {
        const string phone = "(223) 456-7890";
        const string email = "gary@somedomain.org";
        const string cc = "4000-3000-2000-1000";
        const string inspectFilePath = "test.txt";
        const string inspectStringValue = "hi my phone number is (223) 456-7890 and my email address is " +
            "gary@somedomain.org. You'll find my credit card number is 4000-3000-2000-1000!";

        const string ident = "111223333";
        private string deidFpeStringValue = "Please de-identify the following identifier: 111223333";
        private Regex deidFpeResultRegex = 
            new Regex("Please de-identify the following identifier: TOKEN\\(\\d+\\):(?<ident>.{9})");
        private Regex alphanumRegex = new Regex("[a-zA-Z0-9]*");
        private Regex hexRegex = new Regex("[0-9A-F]*");
        private Regex numRegex = new Regex("\\d*");
        private Regex alphanumUcRegex = new Regex("[A-Z0-9]*");
        private DlpTestFixture kmsFixture;
        private string ProjectId { get { return kmsFixture.ProjectId; } }

        #region anassri_tests;
        private string ResourcePath = Path.GetFullPath("../../../resources/");
        private string CallingProjectId { get { return kmsFixture.ProjectId; } }
        private string TableProjectId { get { return "nodejs-docs-samples"; } } // TODO make retrieval more idiomatic
        private string KeyName { get { return kmsFixture.KeyName; } }
        private string WrappedKey { get { return kmsFixture.WrappedKey; } }

        // TODO change these
        private string BucketName = "nodejs-docs-samples";
        private string TopicId = "dlp-nyan-2";
        private string SubscriptionId = "nyan-dlp-2";

        // TODO keep these values, but make their retrieval more idiomatic
        private string DatasetId = "integration_tests_dlp";
        private string TableId = "harmful";
        // FYI these values depend on a BQ table in nodejs-docs-samples; we should verify its publicly accessible
        private string QuasiIds = "Age,Gender";
        private string QuasiIdInfoTypes = "AGE,GENDER";
        private string SensitiveAttribute = "Name";
        #endregion

        readonly CommandLineRunner _dlp = new CommandLineRunner()
        {
            VoidMain = Dlp.Main,
            Command = "Dlp"
        };

        public DlpTest(DlpTestFixture fixture)
        {
            kmsFixture = fixture;
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
        }

        [Fact]
        public void TestListInfoTypes()
        {
            // list all info types
            ConsoleOutput outputA = _dlp.Run("listInfoTypes");
            Assert.Contains("US_DEA_NUMBER", outputA.Stdout);
            Assert.Contains("AMERICAN_BANKERS_CUSIP_ID", outputA.Stdout);

            // list info types with a filter
            ConsoleOutput outputB = _dlp.Run(
                "listInfoTypes",
                "-f", "supported_by=RISK_ANALYSIS"
            );
            Assert.Contains("AGE", outputB.Stdout);
            Assert.DoesNotContain("AMERICAN_BANKERS_CUSIP_ID", outputB.Stdout);
        }
        */

        [Fact]
        public void TestInspectString()
        {
            // inspect a string with results
            ConsoleOutput outputA = _dlp.Run(
                "inspectString",
                ProjectId,
                "The name Robert is very common.",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect a string with no results
            ConsoleOutput outputB = _dlp.Run(
                "inspectString",
                ProjectId,
                "She sells sea shells by the sea shore."
            );
            Assert.Contains("No findings", outputB.Stdout);
        }

        [Fact]
        public void TestInspectFile()
        {
            // inspect a text file with results
            ConsoleOutput outputA = _dlp.Run(
                "inspectFile",
                ProjectId,
                ResourcePath + "test.txt",
                "-i", "PERSON_NAME"
            );
            Assert.Contains("PERSON_NAME", outputA.Stdout);

            // inspect an image file with results
            ConsoleOutput outputB = _dlp.Run(
                "inspectFile",
                ProjectId,
                ResourcePath + "test.png",
                "-i", "PHONE_NUMBER,EMAIL_ADDRESS"
            );
            Assert.Contains("PHONE_NUMBER", outputB.Stdout);

            // inspect a file with no results
            ConsoleOutput outputC = _dlp.Run(
                "inspectFile",
                ProjectId,
                ResourcePath + "harmless.txt"
            );
            Assert.Contains("No findings", outputC.Stdout);
        }

        [Fact]
        public void TestDeidMask()
        {
            ConsoleOutput output = _dlp.Run(
                "deidMask",
                ProjectId,
                "'My SSN is 372819127.'",
                "-n", "5",
                "-m", "*"
            );
            Assert.Contains("My SSN is *****9127", output.Stdout);
        }

        [Fact]
        public void TestTemplates()
        {
            // Creation
            string name = $"my-inspect-template-{new Guid()}";
            string displayName = $"My display name {new Guid()}";
            string description = $"My description {new Guid()}";
            string fullName = $"projects/{ProjectId}/inspectTemplates/{name}";

            ConsoleOutput output = _dlp.Run(
                "createInspectTemplate",
                ProjectId,
                name,
                displayName,
                description);

            Assert.Contains("Successfully created template {fullName}", output.Stdout);

            // List
            output = _dlp.Run("listTemplates", ProjectId);
            Assert.Contains($"Template {fullName}", output.Stdout);
            Assert.Contains($"Display Name: {displayName}", output.Stdout);
            Assert.Contains($"Description: {description}", output.Stdout);

            // Deletion
            output = _dlp.Run("deleteTemplate", ProjectId, name);
            Assert.Contains($"Successfully deleted template {fullName}", output.Stdout);
        }

        [Fact]
        public void TestDeidentifyDates()
        {
            string InputPath = ResourcePath + "dates-input.csv";
            string OutputPath = ResourcePath + "resources/dates-shifted.csv";
            string CorrectPath = ResourcePath + "resources/dates-correct.csv";

            ConsoleOutput output = _dlp.Run(
                "deidDateShift",
                ProjectId,
                InputPath,
                OutputPath,
                "50",
                "50",
                "birth_date,register_date",
                "name",
                WrappedKey,
                KeyName);

            Assert.Equal(
                File.ReadAllText(OutputPath),
                File.ReadAllText(CorrectPath));
        }

        [Fact]
        public void TestDeidReidFpe()
        {
            string data = "'My SSN is 372819127'";
            string alphabet = "Numeric";

            // Deid
            ConsoleOutput deidOutput = _dlp.Run("deidFpe", CallingProjectId, data, KeyName, WrappedKey, alphabet);
            Assert.Matches(new Regex("My SSN is TOKEN\\(9\\):\\d+"), deidOutput.Stdout);

            // Reid
            ConsoleOutput reidOutput = _dlp.Run("reidFpe", CallingProjectId, data, KeyName, WrappedKey, alphabet);
            Assert.Contains(data, reidOutput.Stdout);
        }

        [Fact]
        public void TestTriggers()
        {
            string triggerId = $"my-csharp-test-trigger-{Guid.NewGuid()}";
            string fullTriggerId = $"projects/{CallingProjectId}/jobTriggers/{triggerId}";
            string displayName = $"My trigger display name {Guid.NewGuid()}";
            string description = $"My trigger description {Guid.NewGuid()}";

            // Create
            ConsoleOutput createOutput = _dlp.Run(
                "createJobTrigger",
                CallingProjectId,
                "-i", "PERSON_NAME,US_ZIP",
                BucketName,
                "1",
                "-l", "Unlikely",
                "-m", "0",
                "-t", triggerId,
                "-n", displayName,
                "-d", description);
            Assert.Contains($"Successfully created trigger {fullTriggerId}", createOutput.Stdout);

            // List
            ConsoleOutput listOutput = _dlp.Run("listJobTriggers", CallingProjectId);
            Assert.Contains($"Name: {fullTriggerId}", listOutput.Stdout);
            Assert.Contains($"Display Name: {displayName}", listOutput.Stdout);
            Assert.Contains($"Description: {description}", listOutput.Stdout);

            // Delete
            ConsoleOutput deleteOutput = _dlp.Run("deleteJobTrigger", fullTriggerId);
            Assert.Contains($"Successfully deleted trigger {fullTriggerId}", deleteOutput.Stdout);
        }

        [Fact]
        public void TestInspectTemplates()
        {
            // Creation
            string templateId = $"my-inspect-template-{Guid.NewGuid()}";
            string displayName = $"'My display name {Guid.NewGuid()}'";
            string description = $"'My description {Guid.NewGuid()}'";

            ConsoleOutput output = _dlp.Run("createInspectTemplate", CallingProjectId, templateId, displayName, description);
            Assert.Contains("name: ", output.Stdout);
            string name = $"projects/{CallingProjectId}/inspectTemplates/{templateId}";

            // List
            output = _dlp.Run("listTemplates", CallingProjectId);
            Assert.Contains("Inspect Template Info:", output.Stdout);
            Assert.Contains($"Display Name: {displayName}", output.Stdout);
            Assert.Contains($"Description: {description}", output.Stdout);

            // Deletion
            output = _dlp.Run("deleteTemplate", CallingProjectId, name);
            Assert.Contains($"Successfully deleted template {name}", output.Stdout);
        }

        [Fact]
        public void TestNumericalStats()
        {
            ConsoleOutput output = _dlp.Run(
                "numericalStats",
                CallingProjectId,
                TableProjectId,
                DatasetId,
                TableId,
                TopicId,
                SubscriptionId,
                "Age"
            );

            Assert.Matches(new Regex("Value Range: \\[\\d+, \\d+\\]"), output.Stdout);
            Assert.Matches(new Regex("Value at \\d+% quantile: \\d+"), output.Stdout);
        }

        [Fact]
        public void TestCategoricalStats()
        {
            ConsoleOutput output = _dlp.Run(
                "categoricalStats",
                CallingProjectId,
                TableProjectId,
                DatasetId,
                TableId,
                TopicId,
                SubscriptionId,
                "Gender"
            );

            Assert.Matches(new Regex("Least common value occurs \\d+ time\\(s\\)"), output.Stdout);
            Assert.Matches(new Regex("Most common value occurs \\d+ time\\(s\\)"), output.Stdout);
            Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
        }

        [Fact]
        public void TestKAnonymity()
        {
            ConsoleOutput output = _dlp.Run(
                "kAnonymity",
                CallingProjectId,
                TableProjectId,
                DatasetId,
                TableId,
                TopicId,
                SubscriptionId,
                QuasiIds
            );

            Assert.Matches(new Regex("Quasi-ID values: \\[\\d{2},Female\\]"), output.Stdout);
            Assert.Matches(new Regex("Class size: \\d"), output.Stdout);
            Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
        }

        [Fact]
        public void TestLDiversity()
        {
            ConsoleOutput output = _dlp.Run(
                "lDiversity",
                CallingProjectId,
                TableProjectId,
                DatasetId,
                TableId,
                TopicId,
                SubscriptionId,
                QuasiIds,
                SensitiveAttribute
            );

            Assert.Matches(new Regex("Quasi-ID values: \\[\\d{2},Female\\]"), output.Stdout);
            Assert.Matches(new Regex("Class size: \\d"), output.Stdout);
            Assert.Matches(new Regex("Sensitive value James occurs \\d time\\(s\\)"), output.Stdout);
            Assert.Matches(new Regex("\\d+ unique value\\(s\\) total"), output.Stdout);
        }

        [Fact]
        public void TestKMap()
        {
            ConsoleOutput output = _dlp.Run(
                "kMap",
                CallingProjectId,
                TableProjectId,
                DatasetId,
                TableId,
                TopicId,
                SubscriptionId,
                QuasiIds,
                QuasiIdInfoTypes,
                "US"
            );

            Assert.Matches(new Regex("Anonymity range: \\[\\d, \\d\\]"), output.Stdout);
            Assert.Matches(new Regex("Size: \\d"), output.Stdout);
            Assert.Matches(new Regex("Values: \\[\\d{2},Female,US\\]"), output.Stdout);
        }

        [Fact]
        public void TestJobs()
        {
            Regex dlpJobRegex = new Regex("projects/.*/dlpJobs/r-\\d+");

            // List
            ConsoleOutput listOutput = _dlp.Run("listJobs", CallingProjectId, "state=DONE", "RiskAnalysisJob");
            Assert.Matches(dlpJobRegex, listOutput.Stdout);

            // Delete
            string jobName = dlpJobRegex.Match(listOutput.Stdout).Value;
            ConsoleOutput deleteOutput = _dlp.Run("deleteJob", jobName);
            Assert.Contains($"Successfully deleted job {jobName}", deleteOutput.Stdout);
        }
    }
}
