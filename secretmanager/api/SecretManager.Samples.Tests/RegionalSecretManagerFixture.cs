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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using System;
using System.Text;
using Xunit;

[CollectionDefinition(nameof(RegionalSecretManagerFixture))]
public class RegionalSecretManagerFixture : IDisposable, ICollectionFixture<RegionalSecretManagerFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; }
    public Secret Secret { get; }
    public Secret SecretToDelete { get; }
    public Secret SecretToDeleteWithEtag { get; }
    public Secret SecretWithVersions { get; }
    public SecretName SecretForQuickstartName { get; }
    public SecretName SecretToCreateName { get; }
    public SecretVersion SecretVersion { get; }
    public SecretVersion SecretVersionToDelete { get; }
    public SecretVersion SecretVersionToDeleteWithEtag { get; }
    public SecretVersion SecretVersionToDestroyWithEtag { get; }
    public SecretVersion SecretVersionToDestroy { get; }
    public SecretVersion SecretVersionToDisable { get; }
    public SecretVersion SecretVersionToEnable { get; }

    public RegionalSecretManagerFixture()
    {
        Console.WriteLine("Hello World!");
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Get LocationId (e.g., "us-central1")
        LocationId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_LOCATION") ?? "us-west1";

        // Required for testing regional samples
        Secret = CreateSecret(RandomId());
        SecretToDelete = CreateSecret(RandomId());
        SecretToDeleteWithEtag = CreateSecret(RandomId());
        SecretWithVersions = CreateSecret(RandomId());
        SecretForQuickstartName = SecretName.FromProjectLocationSecret(ProjectId, LocationId, RandomId());
        SecretToCreateName = SecretName.FromProjectLocationSecret(ProjectId, LocationId, RandomId());

        SecretVersionToDelete = AddSecretVersion(SecretToDelete);
        SecretVersionToDeleteWithEtag = AddSecretVersion(SecretToDeleteWithEtag);

        SecretVersion = AddSecretVersion(SecretWithVersions);
        SecretVersionToDestroy = AddSecretVersion(SecretWithVersions);
        SecretVersionToDestroyWithEtag = AddSecretVersion(SecretWithVersions);
        SecretVersionToDisable = AddSecretVersion(SecretWithVersions);
        SecretVersionToEnable = AddSecretVersion(SecretWithVersions);
        DisableSecretVersion(SecretVersionToEnable);

        // Required for regional samples
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
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{LocationId}.rep.googleapis.com"
        }.Build();

        LocationName locationName = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret { };

        return client.CreateSecret(locationName, secretId, secret);
    }

    private SecretVersion AddSecretVersion(Secret secret)
    {
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{LocationId}.rep.googleapis.com"
        }.Build();

        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return client.AddSecretVersion(secret.SecretName, payload);
    }

    private void DeleteSecret(SecretName name)
    {
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{LocationId}.rep.googleapis.com"
        }.Build();

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
        // Create the Secret Manager Client with the regional endpoint.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{LocationId}.rep.googleapis.com"
        }.Build();

        client.DisableSecretVersion(version.SecretVersionName);
    }
}
