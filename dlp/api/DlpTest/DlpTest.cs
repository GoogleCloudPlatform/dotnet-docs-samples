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
        public CryptoKey Key;
        public string CipherFilePath = "kmsCipher.key";
        private readonly CloudKMSService kms;
        private KeyRing keyRing;

        public DlpTestFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Cloud Key Management Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudKMSService.Scope.CloudPlatform
                });
            }
            // Instantiate the Cloud Key Management Service API.
            kms = new CloudKMSService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            // Create the test key ring and key if they do not exist.
            var parent = $"projects/{ProjectId}/locations/global";
            var keyRingName = $"{ProjectId}-test";

            keyRing = kms.Projects.Locations.KeyRings
                .List(parent)
                .Execute()
                .KeyRings
                .FirstOrDefault(ring => ring.Name.Contains(keyRingName));
            if (keyRing == null)
            {
                keyRing = new KeyRing
                {
                    Name = keyRingName
                };
                var createKeyRingRequest = kms.Projects.Locations.KeyRings.Create(keyRing, parent);
                createKeyRingRequest.KeyRingId = keyRingName;
                keyRing = createKeyRingRequest.Execute();
            }
            Key = kms.Projects.Locations.KeyRings.CryptoKeys
                .List(keyRing.Name)
                .Execute()
                .CryptoKeys
                ?.FirstOrDefault();
            if (Key == null)
            {
                Key = new CryptoKey
                {
                    Purpose = "ENCRYPT_DECRYPT"
                };
                var createKeyRequest = kms.Projects.Locations.KeyRings.CryptoKeys.Create(Key, keyRing.Name);
                createKeyRequest.CryptoKeyId = keyRingName;
                Key = createKeyRequest.Execute();
            }

            // Write the encrypted key file with the new key.
            var plainBytes = File.ReadAllBytes("kmsPlain.txt");
            var encryptResponse = kms.Projects.Locations.KeyRings.CryptoKeys
                .Encrypt(new EncryptRequest
                {
                    Plaintext = Convert.ToBase64String(plainBytes)
                }, Key.Name)
                .Execute();
            File.WriteAllBytes(CipherFilePath, Convert.FromBase64String(encryptResponse.Ciphertext));
        }
    }

    public class DlpTest : IClassFixture<DlpTestFixture>
    {
        const string phone = "(223) 456-7890";
        const string email = "gary@somedomain.org";
        const string cc = "4000-3000-2000-1000";
        const string inspectFilePath = "test.txt";
        const string inspectStringValue = "hi my phone number is (223) 456-7890 and my email address is " +
            "gary@somedomain.org. You'll find my credit card number is 4000-3000-2000-1000!";

        const string ident = "111223333";
        private string deidFpeStringValue = "Please de-identify the following identifier: 111223333";
        private Regex deidFpeResultRegex = new Regex("Please de-identify the following identifier: (?<ident>.{9})");
        private Regex alphanumRegex = new Regex("[a-zA-Z0-9]*");
        private Regex hexRegex = new Regex("[0-9A-F]*");
        private Regex numRegex = new Regex("\\d*");
        private Regex alphanumUcRegex = new Regex("[A-Z0-9]*");
        private DlpTestFixture kmsFixture;
        private string ProjectId { get { return kmsFixture.ProjectId; } }

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

        private string ProcessDeid(List<string> args, string value)
        {
            var output = _dlp.Run(args.ToArray());
            var result = output.Stdout.Replace("Deidentified content: ", "").Trim();
            Assert.DoesNotContain(phone, result);
            Assert.DoesNotContain(email, result);
            Assert.DoesNotContain(cc, result);
            Assert.DoesNotContain(ident, result);
            Assert.Equal(value.Length, result.Length);
            return result;
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
            string result = ProcessDeid(args, inspectStringValue);

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
                kmsFixture.Key.Name,
                kmsFixture.CipherFilePath
            };
            if (alphabet != null)
            {
                args.Add("-a");
                args.Add(alphabet);
            }

            string result = ProcessDeid(args, deidFpeStringValue);
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
            AssertMatch(match, replacementRegex, "ident", ident.Length);

            // Test Reid with output.
            // Currently only works with NUMERIC alphabet type, because the Deidentification
            // still looks for infotype, and the only InfoType that works as expected is
            // US_SOCIAL_SECURITY_NUMBER, which must be entirely numeric to qualify
            // both for inspection and deidentification under the same alphabet.
            if (alphabet != "num")
            {
                return;
            }
            args[0] = "reidFpe";
            args[2] = result;

            var output = _dlp.Run(args.ToArray());
            var reidResult = output.Stdout.Replace("Reidentified content: ", "").Trim();
            Assert.Equal(deidFpeStringValue, reidResult);
        }

        private static void AssertMatch(Match match, Regex replacementRegex, string groupName, int length)
        {
            var replacementMatch = replacementRegex.Match(match.Groups[groupName].Value);
            Assert.True(replacementMatch.Success);
            Assert.Equal(length, replacementMatch.Value.Length);
        }
    }
}
