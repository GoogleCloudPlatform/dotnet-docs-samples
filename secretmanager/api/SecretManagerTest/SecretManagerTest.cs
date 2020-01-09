// Copyright(c) 2019 Google Inc.
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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1Beta1;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Runs the sample app's methods and tests the outputs
    /// </summary>
    public class SampleTests
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _secretManager = new CommandLineRunner()
        {
            VoidMain = SecretManagerSample.Main,
            Command = "SecretManagerSample"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _secretManager.Run(args);
        }

        public SampleTests()
        {
            if (s_projectId == null || s_projectId == "")
            {
                throw new Exception("missing GOOGLE_PROJECT_ID");
            }
        }

        [Fact]
        public void TestCreateAddAccessDelete()
        {
            string secretId = $"csharp-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            Run("create", s_projectId, secretId);
            Run("add-version", s_projectId, secretId);

            var accessVersionOut = Run("access-version", s_projectId, secretId, "1");
            Assert.Contains("my super secret data", accessVersionOut.Stdout);

            Run("delete", s_projectId, secretId);
        }

        [Fact]
        public void TestEnableDisableDestroy()
        {
            string secretId = $"csharp-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            Run("create", s_projectId, secretId);
            Run("add-version", s_projectId, secretId);

            var disableOut = Run("disable-version", s_projectId, secretId, "1");
            Assert.Contains("Disabled secret version", disableOut.Stdout);

            var enableOut = Run("enable-version", s_projectId, secretId, "1");
            Assert.Contains("Enabled secret version", enableOut.Stdout);

            var destroyOut = Run("destroy-version", s_projectId, secretId, "1");
            Assert.Contains("Destroyed secret version", destroyOut.Stdout);

            Run("delete", s_projectId, secretId);
        }

        [Fact]
        public void TestList()
        {
            string ts = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string secret1Id = $"csharp-list-{ts}-1";
            string secret2Id = $"csharp-list-{ts}-2";
            string secret3Id = $"csharp-list-{ts}-3";


            Run("create", s_projectId, secret1Id);
            Run("create", s_projectId, secret2Id);
            Run("create", s_projectId, secret3Id);

            Run("add-version", s_projectId, secret1Id);
            Run("add-version", s_projectId, secret1Id);
            Run("add-version", s_projectId, secret1Id);

            var secretsOut = Run("list", s_projectId);
            Assert.Contains($"{ts}", secretsOut.Stdout);

            var versionsOut = Run("list-versions", s_projectId, secret1Id);
            Assert.Contains($"{secret1Id}/versions/1", versionsOut.Stdout);

            Run("delete", s_projectId, secret1Id);
            Run("delete", s_projectId, secret2Id);
            Run("delete", s_projectId, secret3Id);
        }
    }

    /// <summary>
    /// Runs the quickstart and tests behavior
    /// </summary>
    public class QuickStartTests
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "QuickStart"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _quickStart.Run(args);
        }

        public QuickStartTests()
        {
            if (s_projectId == null || s_projectId == "")
            {
                throw new Exception("missing GOOGLE_PROJECT_ID");
            }
        }

        [Fact]
        public void TestRun()
        {
            string ts = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            string secretId = $"csharp-quickstart-{ts}-1";

            // Run quickstart
            var output = _quickStart.Run(s_projectId, secretId);

            // Verify output
            Assert.Equal(0, output.ExitCode);
            Assert.Contains($"Plaintext: my super secret data", output.Stdout);

            // Delete the secret created by quickstart
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            client.DeleteSecret(new DeleteSecretRequest
            {
                SecretName = new SecretName(s_projectId, secretId),
            });
        }
    }
}
