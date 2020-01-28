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
using System.Text;
using Xunit;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1Beta1;
using Google.Protobuf;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Runs the sample app's methods and tests the outputs
    /// </summary>
    public class SampleTests : IClassFixture<SecretsFixture>
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private static readonly string s_iamUser = $"user:sethvargo@google.com";
        private static readonly SecretManagerServiceClient s_client = SecretManagerServiceClient.Create();

        protected SecretsFixture secretsFixture;

        readonly CommandLineRunner _secretManager = new CommandLineRunner()
        {
            VoidMain = SecretManagerSample.Main,
            Command = "SecretManagerSample"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _secretManager.Run(args);
        }

        public SampleTests(SecretsFixture secretsFixture)
        {
            if (String.IsNullOrEmpty(s_projectId))
            {
                throw new Exception("missing GOOGLE_PROJECT_ID");
            }

            this.secretsFixture = secretsFixture;
        }

        [Fact]
        public void TestAccessSecretVersion()
        {
            var name = secretsFixture.SecretVersion.SecretVersionName;
            var output = Run("access-version", name.ProjectId, name.SecretId, name.SecretVersionId);
            Assert.Contains("my super secret data", output.Stdout);
        }

        [Fact]
        public void TestAddSecretVersion()
        {
            var name = secretsFixture.SecretWithVersions.SecretName;
            var output = Run("add-version", name.ProjectId, name.SecretId);
            Assert.Contains("Added secret version", output.Stdout);
        }

        [Fact]
        public void TestCreateSecret()
        {
            var name = secretsFixture.SecretToCreateName;
            var output = Run("create", name.ProjectId, name.SecretId);
            Assert.Contains("Created secret", output.Stdout);

            var secret = s_client.GetSecret(new GetSecretRequest
            {
                SecretName = name
            });
            Assert.Equal(name.SecretId, secret.SecretName.SecretId);
        }

        [Fact]
        public void TestDeleteSecret()
        {
            var name = secretsFixture.SecretToDelete.SecretName;
            var output = Run("delete", name.ProjectId, name.SecretId);
            Assert.Contains($"Deleted secret {name.SecretId}", output.Stdout);

            Assert.Throws<Grpc.Core.RpcException>(() =>
            {
                s_client.GetSecret(new GetSecretRequest { SecretName = name });
            });
        }

        [Fact]
        public void TestDestroySecretVersion()
        {
            var name = secretsFixture.SecretVersionToDestroy.SecretVersionName;
            var output = Run("destroy-version", name.ProjectId, name.SecretId, name.SecretVersionId);
            Assert.Contains($"Destroyed secret version {name}", output.Stdout);

            var version = s_client.GetSecretVersion(new GetSecretVersionRequest
            {
                SecretVersionName = name
            });
            Assert.Equal(SecretVersion.Types.State.Destroyed, version.State);
        }

        [Fact]
        public void TestDisableSecretVersion()
        {
            var name = secretsFixture.SecretVersionToDisable.SecretVersionName;
            var disableOut = Run("disable-version", name.ProjectId, name.SecretId, name.SecretVersionId);
            Assert.Contains($"Disabled secret version {name}", disableOut.Stdout);

            var version = s_client.GetSecretVersion(new GetSecretVersionRequest
            {
                SecretVersionName = name
            });
            Assert.Equal(SecretVersion.Types.State.Disabled, version.State);
        }

        [Fact]
        public void TestEnableSecretVersion()
        {
            var name = secretsFixture.SecretVersionToEnable.SecretVersionName;
            var enableOut = Run("enable-version", name.ProjectId, name.SecretId, name.SecretVersionId);
            Assert.Contains($"Enabled secret version {name}", enableOut.Stdout);

            var version = s_client.GetSecretVersion(new GetSecretVersionRequest
            {
                SecretVersionName = name
            });
            Assert.Equal(SecretVersion.Types.State.Enabled, version.State);
        }

        [Fact]
        public void TestGetSecretVersion()
        {
            var name = secretsFixture.SecretVersion.SecretVersionName;
            var output = Run("get-version", name.ProjectId, name.SecretId, name.SecretVersionId);
            Assert.Contains($"Secret version {name}, state Enabled", output.Stdout);
        }

        [Fact]
        public void TestGetSecret()
        {
            var name = secretsFixture.Secret.SecretName;
            var output = Run("get", name.ProjectId, name.SecretId);
            Assert.Contains($"Secret {name}, replication Automatic", output.Stdout);
        }

        [Fact]
        public void TestIAMGrantAccess()
        {
            var name = secretsFixture.Secret.SecretName;
            var output = Run("iam-grant-access", name.ProjectId, name.SecretId, $"{s_iamUser}");
            Assert.Contains($"Updated IAM policy for {name.SecretId}", output.Stdout);
        }

        [Fact]
        public void TestIAMRevokeAccess()
        {
            var name = secretsFixture.Secret.SecretName;
            var output = Run("iam-revoke-access", name.ProjectId, name.SecretId, $"{s_iamUser}");
            Assert.Contains($"Updated IAM policy for {name.SecretId}", output.Stdout);
        }

        [Fact]
        public void TestListSecretVersions()
        {
            var name = secretsFixture.SecretWithVersions.SecretName;
            var output = Run("list-versions", name.ProjectId, name.SecretId);
            Assert.Contains($"Secret version", output.Stdout);
        }

        [Fact]
        public void TestListSecrets()
        {
            var name = secretsFixture.Secret.SecretName;
            var output = Run("list", name.ProjectId);
            Assert.Contains($"Secret {name}", output.Stdout);
        }

        [Fact]
        public void TestUpdateSecret()
        {
            var name = secretsFixture.Secret.SecretName;
            var output = Run("update", name.ProjectId, name.SecretId);
            Assert.Contains($"Updated secret {name}", output.Stdout);

            var secret = s_client.GetSecret(new GetSecretRequest { SecretName = name });
            Assert.Equal("rocks", secret.Labels["secretmanager"]);
        }
    }
}
