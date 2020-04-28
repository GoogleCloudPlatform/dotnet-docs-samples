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
using System.Text;
using Xunit;

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;

[CollectionDefinition(nameof(SecretManagerFixture))]
public class SecretManagerFixture : IDisposable, ICollectionFixture<SecretManagerFixture>
{
    public string ProjectId { get; }
    public Secret Secret { get; }
    public Secret SecretToDelete { get; }
    public Secret SecretWithVersions { get; }
    public SecretName SecretForQuickstartName { get; }
    public SecretName SecretToCreateName { get; }
    public SecretVersion SecretVersion { get; }
    public SecretVersion SecretVersionToDestroy { get; }
    public SecretVersion SecretVersionToDisable { get; }
    public SecretVersion SecretVersionToEnable { get; }

    public SecretManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        Secret = CreateSecret(RandomId());
        SecretToDelete = CreateSecret(RandomId());
        SecretWithVersions = CreateSecret(RandomId());
        SecretForQuickstartName = new SecretName(ProjectId, RandomId());
        SecretToCreateName = new SecretName(ProjectId, RandomId());

        SecretVersion = AddSecretVersion(SecretWithVersions);
        SecretVersionToDestroy = AddSecretVersion(SecretWithVersions);
        SecretVersionToDisable = AddSecretVersion(SecretWithVersions);
        SecretVersionToEnable = AddSecretVersion(SecretWithVersions);
        DisableSecretVersion(SecretVersionToEnable);
    }

    public void Dispose()
    {
        DeleteSecret(Secret.SecretName);
        DeleteSecret(SecretForQuickstartName);
        DeleteSecret(SecretToCreateName);
        DeleteSecret(SecretToDelete.SecretName);
        DeleteSecret(SecretWithVersions.SecretName);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Secret CreateSecret(string secretId)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        ProjectName projectName = new ProjectName(ProjectId);

        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
        };

        return client.CreateSecret(projectName, secretId, secret);
    }

    private SecretVersion AddSecretVersion(Secret secret)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return client.AddSecretVersion(secret.SecretName, payload);
    }

    private void DeleteSecret(SecretName name)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        try
        {
            client.DeleteSecret(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted
        }
    }

    private void DisableSecretVersion(SecretVersion version)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        client.DisableSecretVersion(version.SecretVersionName);
    }
}
