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
        private string QuasiIds = "Age,Gender,RegionCode";
        private string QuasiIdInfoTypes = "AGE,GENDER,REGION_CODE";
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

        private void AssertPhoneEmailCC(
            ConsoleOutput output,
            bool checkPhone = false,
            bool checkEmail = false,
            bool checkCc = false)
        {
            var results = 0;
            if (checkPhone)
            {
                results++;
            }
            if (checkEmail)
            {
                results++;
            }
            if (checkCc)
            {
                results++;
            }
            Assert.Contains($"Found {results} results", output.Stdout);
            if (checkPhone)
            {
                Assert.Contains(phone, output.Stdout);
            }
            if (checkEmail)
            {
                Assert.Contains(email, output.Stdout);
            }
            if (checkCc)
            {
                Assert.Contains(cc, output.Stdout);
            }
        }

        /// <summary>
        /// With default params, Dlp will find matches of all info types and likelihoods, including quote info.
        /// </summary>
        [Theory]
        [InlineData("inspectString", inspectStringValue)]
        [InlineData("inspectFile", inspectFilePath)]
        public void TestInspectStringFilterInfoTypes(string verb, string value)
        {
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value), checkPhone: true, checkEmail: true, checkCc: true);
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-i", "PHONE_NUMBER,EMAIL_ADDRESS,CREDIT_CARD_NUMBER"),
                checkPhone: true,
                checkEmail: true,
                checkCc: true);
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-i", "PHONE_NUMBER,EMAIL_ADDRESS"),
                checkPhone: true,
                checkEmail: true);
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-i", "PHONE_NUMBER,CREDIT_CARD_NUMBER"),
                checkPhone: true,
                checkCc: true);
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-i", "EMAIL_ADDRESS,CREDIT_CARD_NUMBER"),
                checkEmail: true,
                checkCc: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-i", "PHONE_NUMBER"), checkPhone: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-i", "EMAIL_ADDRESS"), checkEmail: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-i", "CREDIT_CARD_NUMBER"), checkCc: true);
        }

        /// <summary>
        /// Dlp evaluates this content string as having
        /// PHONE_NUMBER - VERY_LIKELY
        /// EMAIL_ADDRESS - LIKELY
        /// CREDIT_CARD_NUMBER - VERY_LIKELY
        /// </summary>
        [Theory]
        [InlineData("inspectString", inspectStringValue)]
        [InlineData("inspectFile", inspectFilePath)]
        public void TestInspectStringFilterLikelihood(string verb, string value)
        {
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-l", "4"),
                checkPhone: true,
                checkEmail: true,
                checkCc: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-l", "5"), checkPhone: true, checkCc: true);
        }

        /// <summary>
        /// Dlp finds the matches in the order phone -> email -> credit card
        /// </summary>
        [Theory]
        [InlineData("inspectString", inspectStringValue)]
        [InlineData("inspectFile", inspectFilePath)]
        public void TestInspectStringFilterMaxResults(string verb, string value)
        {
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-m", "0"),
                checkPhone: true,
                checkEmail: true,
                checkCc: true);
            AssertPhoneEmailCC(
                _dlp.Run(verb, ProjectId, value, "-m", "3"),
                checkPhone: true,
                checkEmail: true,
                checkCc: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-m", "2"), checkPhone: true, checkEmail: true);
            AssertPhoneEmailCC(_dlp.Run(verb, ProjectId, value, "-m", "1"), checkPhone: true);
        }

        [Theory]
        [InlineData()]
        [InlineData("y")]
        [InlineData(null, 3)]
        [InlineData(null, 4, true)]
        [InlineData("x", 0, false)]
        [InlineData("Q", 5, false)]
        [InlineData("h", 5, true)]
        [InlineData("B", 9001, false)]
        [InlineData("replace this", 9001, true)]
        public void TestDeIdentifyMask(string mask = null, int num = -1, bool reverse = false)
        {
            var args = new List<string>
            {
                "deidMask",
                ProjectId,
                inspectStringValue
            };
            if (mask != null)
            {
                args.Add("-m");
                args.Add(mask);
            }
            if (num >= 0)
            {
                args.Add("-n");
                args.Add($"{num}");
            }
            if (reverse)
            {
                args.Add("-r");
            }
            var output = _dlp.Run(args.ToArray());
            var result = output.Stdout.Replace("Deidentified content: ", "").Trim();
            Assert.DoesNotContain(phone, result);
            Assert.DoesNotContain(email, result);
            Assert.DoesNotContain(cc, result);
            Assert.DoesNotContain(ident, result);
            Assert.Equal(inspectStringValue.Length, result.Length);

            string expected;
            // Regardless of mask provided, only first character is used to replace deid'd content.
            var maskingChar = mask != null ? mask.ToCharArray()[0] : 'x';
            if (num <= 0)
            {
                expected = inspectStringValue
                    .Replace(phone, "".PadRight(phone.Length, maskingChar))
                    .Replace(email, "".PadRight(email.Length, maskingChar))
                    .Replace(cc, "".PadRight(cc.Length, maskingChar));
            }
            else
            {
                if (reverse)
                {
                    expected = inspectStringValue
                        .Replace(
                            phone.Substring(
                                Math.Max(0, phone.Length - num),
                                Math.Min(phone.Length, num)),
                            "".PadRight(
                                Math.Min(phone.Length, num),
                                maskingChar))
                        .Replace(
                            email.Substring(
                                Math.Max(0, email.Length - num),
                                Math.Min(email.Length, num)),
                            "".PadRight(
                                Math.Min(email.Length, num),
                                maskingChar))
                        .Replace(
                            cc.Substring(
                                Math.Max(0, cc.Length - num),
                                Math.Min(cc.Length, num)),
                            "".PadRight(
                                Math.Min(cc.Length, num),
                                maskingChar));
                }
                else
                {
                    expected = inspectStringValue
                        .Replace(
                            phone.Substring(
                                0,
                                Math.Min(phone.Length, num)),
                            "".PadRight(
                                Math.Min(phone.Length, num),
                                maskingChar))
                        .Replace(
                            email.Substring(
                                0,
                                Math.Min(email.Length, num)),
                            "".PadRight(
                                Math.Min(email.Length, num),
                                maskingChar))
                        .Replace(
                            cc.Substring(
                                0,
                                Math.Min(cc.Length, num)),
                            "".PadRight(
                                Math.Min(cc.Length, num),
                                maskingChar));
                }
            }
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("an")]
        [InlineData("hex")]
        [InlineData("num")]
        [InlineData("an-uc")]
        public void TestDeidFpe(string alphabet = null)
        {
            var args = new List<string>
            {
                "deidFpe",
                ProjectId,
                deidFpeStringValue,
                kmsFixture.KeyName,
                kmsFixture.WrappedKey
            };
            if (alphabet != null)
            {
                args.Add("-a");
                args.Add(alphabet);
            }

            var output = _dlp.Run(args.ToArray());
            var result = output.Stdout.Replace("Deidentified content: ", "").Trim();
            Assert.DoesNotContain(ident, result);

            var match = deidFpeResultRegex.Match(result);
            Assert.True(match.Success);

            Regex replacementRegex;
            switch (alphabet)
            {
                default:
                    replacementRegex = alphanumRegex;
                    break;
                case "an":
                    replacementRegex = alphanumRegex;
                    break;
                case "hex":
                    replacementRegex = hexRegex;
                    break;
                case "num":
                    replacementRegex = numRegex;
                    break;
                case "an-uc":
                    replacementRegex = alphanumUcRegex;
                    break;
            }
            var replacementMatch = replacementRegex.Match(match.Groups["ident"].Value);
            Assert.True(replacementMatch.Success);
            Assert.Equal(ident.Length, replacementMatch.Value.Length);

            // Test Reid with output.
            args[0] = "reidFpe";
            args[2] = result;

            var reidOutput = _dlp.Run(args.ToArray());
            var reidResult = reidOutput.Stdout.Replace("Reidentified content: ", "").Trim();
            Assert.Equal(deidFpeStringValue, reidResult);
        }

        [Fact]
        public void TestTemplates()
        {
            // Creation
            ConsoleOutput output = _dlp.Run("createInspectTemplate", ProjectId, "testDisplayName", "test description");
            Assert.Contains("name: ", output.Stdout);
            int startPos = output.Stdout.IndexOf("name: ") + 6;
            int endPos = output.Stdout.IndexOf(",");
            string name = output.Stdout.Substring(startPos, endPos-startPos);

            // List
            output = _dlp.Run("listTemplates", ProjectId);
            Assert.Contains("Inspect Template Info:", output.Stdout);

            // Deletion
            output = _dlp.Run("deleteTemplate", ProjectId, name);
            Assert.Contains(" was deleted", output.Stdout);
        }
    }

    // TODO reconcile these with the "complex" tests above
    public partial class DlpTest : IClassFixture<DlpTestFixture> {
        
        [Fact]
        public void TestDeidentifyMask()
        {
            ConsoleOutput output = _dlp.Run(
                "deidMask",
                CallingProjectId,
                "My SSN is 372819127.",
                "-n", "5");

            Assert.Contains("My SSN is xxxxx9127", output.Stdout);
        }

        [Fact]
        public void TestDeidentifyDates()
        {
            Assert.False(true);
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
