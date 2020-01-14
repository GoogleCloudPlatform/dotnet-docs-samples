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
    /// Fixture for secrets used in all tests.
    /// <summary>
    public class SecretsFixture : IDisposable
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly SecretManagerServiceClient _client = SecretManagerServiceClient.Create();

        public Secret Secret { get; private set; }
        public Secret SecretToDelete { get; private set; }
        public Secret SecretWithVersions { get; private set; }
        public SecretName SecretToCreateName { get; private set; }
        public SecretVersion SecretVersion { get; private set; }
        public SecretVersion SecretVersionToDestroy { get; private set; }
        public SecretVersion SecretVersionToDisable { get; private set; }
        public SecretVersion SecretVersionToEnable { get; private set; }

        public SecretsFixture()
        {
            if (String.IsNullOrEmpty(s_projectId))
            {
                throw new Exception("missing GOOGLE_PROJECT_ID");
            }

            Secret = CreateSecret();
            SecretToDelete = CreateSecret();
            SecretWithVersions = CreateSecret();
            SecretToCreateName = new SecretName(s_projectId, RandomSecretId());

            SecretVersion = AddSecretVersion(SecretWithVersions);
            SecretVersionToDestroy = AddSecretVersion(SecretWithVersions);
            SecretVersionToDisable = AddSecretVersion(SecretWithVersions);
            SecretVersionToEnable = AddSecretVersion(SecretWithVersions);
            DisableSecretVersion(SecretVersionToEnable);
        }

        public void Dispose()
        {
            DeleteSecret(Secret.SecretName);
            DeleteSecret(SecretToCreateName);
            DeleteSecret(SecretToDelete.SecretName);
            DeleteSecret(SecretWithVersions.SecretName);
        }

        private String RandomSecretId()
        {
            return $"csharp-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
        }

        private Secret CreateSecret()
        {
            var secretId = RandomSecretId();
            var request = new CreateSecretRequest
            {
                ParentAsProjectName = new ProjectName(s_projectId),
                SecretId = secretId,
                Secret = new Secret
                {
                    Replication = new Replication
                    {
                        Automatic = new Replication.Types.Automatic(),
                    },
                },
            };
            return _client.CreateSecret(request);
        }

        private SecretVersion AddSecretVersion(Secret secret)
        {
            var payload = "my super secret data";

            var request = new AddSecretVersionRequest
            {
                ParentAsSecretName = secret.SecretName,
                Payload = new SecretPayload
                {
                    Data = ByteString.CopyFrom(payload, Encoding.UTF8),
                },
            };

            return _client.AddSecretVersion(request);
        }

        private void DeleteSecret(SecretName name)
        {
            try
            {
                _client.DeleteSecret(new DeleteSecretRequest
                {
                    SecretName = name
                });
            }
            catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                // Ignore error - secret was already deleted
            }
        }

        private void DisableSecretVersion(SecretVersion version)
        {
            _client.DisableSecretVersion(new DisableSecretVersionRequest
            {
                SecretVersionName = version.SecretVersionName,
            });
        }
    }
}
