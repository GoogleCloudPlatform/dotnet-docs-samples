// Copyright(c) 2020 Google Inc.
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
using Xunit;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1Beta1;

namespace GoogleCloudSamples
{
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
            if (String.IsNullOrEmpty(s_projectId))
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
