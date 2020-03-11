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

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Google.Cloud.Kms.V1;
using Google.Protobuf;
using Xunit;

namespace GoogleCloudSamples
{
    // <summary>
    /// Runs the sample app's methods and tests the outputs
    // </summary>
    public class CommonTests
    {
        private static readonly string s_projectId = LoadProjectId();

        private static string LoadProjectId()
        {
            const string projectIdEnvVariable = "GOOGLE_PROJECT_ID");
            string projectId = Environment.GetEnvironmentVariable(projectIdEnvVariable);
            if (String.IsNullOrEmpty(projectId))
            {
                throw new ArgumentException($"environment variable {projectIdEnvVariable} is unset");
            }
            return projectId;
        }

        readonly CommandLineRunner _cloudKms = new CommandLineRunner()
        {
            VoidMain = CloudKmsSample.Main,
            Command = "CloudKmsSample"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _cloudKms.Run(args);
        }

        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };

        /// <summary>
        /// Retry action.
        /// Datastore guarantees only eventual consistency.  Many tests write
        /// an entity and then query it afterward, but may not find it immediately.
        /// </summary>
        /// <param name="action"></param>
        private void Eventually(Action action) => _retryRobot.Eventually(action);

        [Fact]
        public void TestListKeyRings()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string keyRing = $"testKeyRing{timeStamp}";
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            string keyRing1 = $"testKeyRing1{timeStamp}";
            var createKeyRing1Output = Run("createKeyRing", s_projectId, "global", keyRing1);
            var listKeyRingsOutput = Run("listKeyRings", s_projectId, "global");
            Assert.Equal(0, listKeyRingsOutput.ExitCode);
            Assert.Contains("keyRings/", listKeyRingsOutput.Stdout);
        }

        [Fact]
        public void TestListCryptoKeys()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            string cryptoKey1 = $"testCryptoKey1{timeStamp}";
            string keyRing = $"testKeyRing{timeStamp}";
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var createCryptoKey1Output = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey1);
            var output = Run("listCryptoKeys", s_projectId, "global", keyRing);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("cryptoKeys/", output.Stdout);
        }

        [Fact]
        public void TestCreateKeyRing()
        {
            string keyRing = $"testKeyRing-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            var output = Run("createKeyRing", s_projectId, "global", keyRing);
            var keyRingDetails = Run("getKeyRing", s_projectId, "global", keyRing);
            Console.WriteLine($"keyRingDetails.Stdout: {keyRingDetails.Stdout}");
            Assert.Contains($"{keyRing}", keyRingDetails.Stdout);
        }

        [Fact]
        public void TestCreateCryptoKey()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            string keyRing = $"testKeyRing{timeStamp}";
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var getCryptoKeyOutput = Run("getCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            Console.WriteLine($"cryptoKeyDetails.Stdout: {getCryptoKeyOutput.Stdout}");
            Assert.Contains($"{cryptoKey}", getCryptoKeyOutput.Stdout);
        }


        [Fact]
        public void TestEncryptData()
        {
            string inFile = @"../../../../test/data/test_file.txt";
            string outFile = @"../../../../test/data/test_file_encrypted.txt";
            string testFile = @"../../../../test/data/test_file_decrypted.txt";
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string keyRing = $"testKeyRing{timeStamp}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var encryptOutput = Run("encrypt", s_projectId, "global", keyRing, cryptoKey, inFile, outFile);
            // Confirm original input file contents are not equal to encrypted file's contents
            Eventually(() =>
            {
                Assert.Equal(0, encryptOutput.ExitCode);
                Assert.False(File.ReadLines(inFile).SequenceEqual(File.ReadLines(outFile)));
            });
            // Confirm original input file contents are equal to decrypted file's contents
            Console.WriteLine("Confirming decrypted contents match original contents...");
            var decryptOutput = Run("decrypt", s_projectId, "global", keyRing, cryptoKey, outFile, testFile);
            Eventually(() =>
            {
                Assert.Equal(0, decryptOutput.ExitCode);
                Assert.True(File.ReadLines(inFile).SequenceEqual(File.ReadLines(testFile)));
            });
            // Delete encrypted & decrypted files to clean up
            File.Delete(outFile);
            File.Delete(testFile);
        }

        [Fact]
        public void TestUpdateCryptoKeyVersionState()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            string keyRing = $"testKeyRing{timeStamp}";
            int versionToTest = 1;
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var getCryptoKeyOutput = Run("getCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            Eventually(() => Assert.Equal(0, getCryptoKeyOutput.ExitCode));
            var disableCryptoKeyVersionOutput = Run("disableCryptoKeyVersion", s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
            using (StringWriter sw = new StringWriter())
            {
                // Redirect Standard Out to StringWriter for test.
                Console.SetOut(sw);
                CloudKmsSample.GetCryptoKeyVersion(
                    s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
                Eventually(() => Assert.Contains("State: Disabled", sw.ToString()));
                // Redirect Standard Out back to Console.Out.
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            var enableCryptoKeyVersionOutput = Run("enableCryptoKeyVersion", s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
            using (StringWriter sw = new StringWriter())
            {
                // Redirect Standard Out to StringWriter for test.
                Console.SetOut(sw);
                CloudKmsSample.GetCryptoKeyVersion(
                    s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
                Eventually(() => Assert.Contains("State: Enabled", sw.ToString()));
                // Redirect Standard Out back to Console.Out.
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }

        [Fact]
        public void TestCryptoKeyVersionDestroyAndRestore()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            string keyRing = $"testKeyRing{timeStamp}";
            int versionToTest = 1;
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var getCryptoKeyOutput = Run("getCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            Eventually(() => Assert.Equal(0, getCryptoKeyOutput.ExitCode));
            var destroyCryptoKeyVersionOutput = Run("destroyCryptoKeyVersion", s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
            using (StringWriter sw = new StringWriter())
            {
                // Redirect Standard Out to StringWriter for test.
                Console.SetOut(sw);
                CloudKmsSample.GetCryptoKeyVersion(
                    s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
                Eventually(() => Assert.Contains("State: DestroyScheduled", sw.ToString()));
                // Redirect Standard Out back to Console.Out.
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
            var restoreCryptoKeyVersionOutput = Run("restoreCryptoKeyVersion", s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
            Eventually(() => Assert.Equal(0, restoreCryptoKeyVersionOutput.ExitCode));
            using (StringWriter sw = new StringWriter())
            {
                // Redirect Standard Out to StringWriter for test.
                Console.SetOut(sw);
                CloudKmsSample.GetCryptoKeyVersion(
                    s_projectId, "global", keyRing, cryptoKey, versionToTest.ToString());
                Eventually(() => Assert.Contains("State: Disabled", sw.ToString()));
                // Redirect Standard Out back to Console.Out.
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
        }

        [Fact]
        public void TestIamAddAndRemoveMembers()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            string keyRing = $"testKeyRing{timeStamp}";
            string testRole = "roles/viewer";
            string testMember = "allUsers";
            var createKeyRingOutput = Run("createKeyRing", s_projectId, "global", keyRing);
            var createCryptoKeyOutput = Run("createCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            var getCryptoKeyOutput = Run("getCryptoKey", s_projectId, "global", keyRing, cryptoKey);
            Eventually(() => Assert.Equal(0, getCryptoKeyOutput.ExitCode));
            // Add role/member to policy.
            var addMemberOutput = Run("addMemberToCryptoKeyPolicy", s_projectId, "global", keyRing, cryptoKey,
                testRole, testMember);
            Eventually(() => Assert.Equal(0, addMemberOutput.ExitCode));
            // Get policy and confirm member is present.
            var getPolicyAddMemberTest = Run("getCryptoKeyIamPolicy", s_projectId, "global", keyRing, cryptoKey);
            Eventually(() => Assert.Equal(0, getPolicyAddMemberTest.ExitCode));
            Assert.True(AssertRoleAndMemberPresent(getPolicyAddMemberTest.Stdout, testRole, testMember));
            // Remove role/member from policy.
            var removeMemberOutput = Run("removeMemberFromCryptoKeyPolicy", s_projectId, "global", keyRing, cryptoKey,
                testRole, testMember);
            Eventually(() => Assert.Equal(0, removeMemberOutput.ExitCode));
            // Get policy and confirm member is absent.
            var getPolicyRemoveMemberTest = Run("getCryptoKeyIamPolicy", s_projectId, "global", keyRing, cryptoKey);
            Eventually(() => Assert.Equal(0, getPolicyRemoveMemberTest.ExitCode));
            Assert.False(AssertRoleAndMemberPresent(getPolicyRemoveMemberTest.Stdout, testRole, testMember));
        }

        private bool AssertRoleAndMemberPresent(string response, string role, string member)
        {
            string[] lines = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool foundRole = false;
            bool foundMember = false;
            foreach (var line in lines)
            {
                if (!foundRole)
                {
                    // Check response for Role
                    if (line.IndexOf(role) != -1)
                    {
                        foundRole = true;
                        continue;
                    }
                }
                else
                {
                    // Check to make sure that next Role was not encountered, otherwise member was not found for role.
                    if (line.IndexOf("Role:") == -1)
                    {
                        // Check response for Member
                        if (line.IndexOf(member) != -1)
                        {
                            foundMember = true;
                            break;
                        }
                    }
                    else
                    {
                        //Next 'Role:' entry in policy was encountered meaning member was not found for role, so exit.
                        break;
                    }
                }
            }
            return (foundRole && foundMember);
        }

        private CryptoKeyVersionName BuildAsymmetricKey(KeyRingName keyRingName, String cryptoKey,
                CryptoKey.Types.CryptoKeyPurpose purpose,
                CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm algorithm)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKey cryptoKeyToCreate = new CryptoKey
            {
                Purpose = purpose,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = algorithm,
                    ProtectionLevel = ProtectionLevel.Software,
                },
            };
            CryptoKey key = client.CreateCryptoKey(keyRingName, cryptoKey, cryptoKeyToCreate);

            CryptoKeyVersionName ckvName = new CryptoKeyVersionName(key.Name, "1");
            CryptoKeyVersion ckv = client.GetCryptoKeyVersion(ckvName);

            // The version is generated asynchronously. Wait for it to be in state Enabled before
            // proceeding. Throw an exception if it hasn't been generated after 5 minutes.
            Task refresher = Task.Run(async () =>
            {
                while (ckv.State != CryptoKeyVersion.Types.CryptoKeyVersionState.Enabled)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    ckv = client.GetCryptoKeyVersion(ckvName);
                }
            });
            if (!refresher.Wait(TimeSpan.FromMinutes(5)))
            {
                throw new TimeoutException($"Timed out waiting for version '{{ckv.Name}}' to become enabled.");
            }

            return ckvName;
        }

        [Fact]
        public void TestAsymmetricSignAndVerify()
        {
            string contentFile = @"../../../../test/data/test_file.txt";
            string signatureFile = @"../../../../test/data/test_file_signature.bin";
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string keyRing = $"testKeyRing{timeStamp}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            KeyRingName keyRingName = new KeyRingName(s_projectId, "global", keyRing);

            Run("createKeyRing", keyRingName.ProjectId, keyRingName.LocationId, keyRingName.KeyRingId);

            CryptoKeyVersionName version = BuildAsymmetricKey(
                keyRingName, cryptoKey, CryptoKey.Types.CryptoKeyPurpose.AsymmetricSign,
                CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.EcSignP256Sha256);

            var signOutput = Run("asymmetricSign", version.ProjectId, version.LocationId, version.KeyRingId,
                    version.CryptoKeyId, version.CryptoKeyVersionId, contentFile, signatureFile);
            Assert.Equal(0, signOutput.ExitCode);

            var verifyOutput = Run("asymmetricVerify", version.ProjectId, version.LocationId, version.KeyRingId,
                    version.CryptoKeyId, version.CryptoKeyVersionId, contentFile, signatureFile);
            Assert.Equal(0, verifyOutput.ExitCode);
            Assert.Contains("verified: True", verifyOutput.Stdout);

            // Delete signature file to clean up
            File.Delete(signatureFile);
        }

        [Fact]
        public void TestAsymmetricEncryptAndDecrypt()
        {
            string plaintextFile = @"../../../../test/data/test_file.txt";
            string ciphertextFile = @"../../../../test/data/test_file_ciphertext.txt";
            string recoveredPlaintextFile = @"../../../../test/data/test_file_recovered_plaintext.txt";
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string keyRing = $"testKeyRing{timeStamp}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            KeyRingName keyRingName = new KeyRingName(s_projectId, "global", keyRing);

            Run("createKeyRing", keyRingName.ProjectId, keyRingName.LocationId, keyRingName.KeyRingId);

            CryptoKeyVersionName version = BuildAsymmetricKey(
                keyRingName, cryptoKey, CryptoKey.Types.CryptoKeyPurpose.AsymmetricDecrypt,
                CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.RsaDecryptOaep2048Sha256);

            var signOutput = Run("asymmetricEncrypt", version.ProjectId, version.LocationId, version.KeyRingId,
                    version.CryptoKeyId, version.CryptoKeyVersionId, plaintextFile, ciphertextFile);
            Assert.Equal(0, signOutput.ExitCode);
            Assert.NotEqual(File.ReadAllBytes(plaintextFile), File.ReadAllBytes(ciphertextFile));

            var verifyOutput = Run("asymmetricDecrypt", version.ProjectId, version.LocationId, version.KeyRingId,
                    version.CryptoKeyId, version.CryptoKeyVersionId, ciphertextFile, recoveredPlaintextFile);
            Assert.Equal(0, verifyOutput.ExitCode);
            Assert.Equal(File.ReadAllBytes(plaintextFile), File.ReadAllBytes(recoveredPlaintextFile));

            // Delete files to clean up
            File.Delete(ciphertextFile);
            File.Delete(recoveredPlaintextFile);
        }

        [Fact]
        public void TestGetPublicKey()
        {
            string timeStamp = $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string keyRing = $"testKeyRing{timeStamp}";
            string cryptoKey = $"testCryptoKey{timeStamp}";
            KeyRingName keyRingName = new KeyRingName(s_projectId, "global", keyRing);

            Run("createKeyRing", keyRingName.ProjectId, keyRingName.LocationId, keyRingName.KeyRingId);

            CryptoKeyVersionName version = BuildAsymmetricKey(
                keyRingName, cryptoKey, CryptoKey.Types.CryptoKeyPurpose.AsymmetricSign,
                CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.EcSignP256Sha256);

            var pubKeyOutput = Run("getPublicKey", version.ProjectId, version.LocationId,
                    version.KeyRingId, version.CryptoKeyId, version.CryptoKeyVersionId);
            Assert.Equal(0, pubKeyOutput.ExitCode);
            Assert.Contains("--BEGIN PUBLIC KEY--", pubKeyOutput.Stdout);
        }
    }

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
            Assert.Contains("keyRings/", output.Stdout);
        }
    }
}
