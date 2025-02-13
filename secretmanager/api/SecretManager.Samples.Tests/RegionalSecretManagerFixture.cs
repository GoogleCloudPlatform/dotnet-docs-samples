// Copyright(c) 2024 Google Inc.
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
    public readonly SecretManagerServiceClient client;
    public string ProjectId { get; }
    public string LocationId { get; }
    public string AnnotationKey { get; }
    public string AnnotationValue { get; }
    public string LabelKey { get; }
    public string LabelValue { get; }
    public Secret Secret { get; }
    public SecretName SecretToCreateName { get; }
    public SecretVersion SecretVersion { get; }


    public RegionalSecretManagerFixture()
    {
        // Get the Google Cloud ProjectId
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Get LocationId (e.g., "us-west1")
        LocationId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_LOCATION") ?? "us-west1";

        // Create the Regional Secret Manager Client
        client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{LocationId}.rep.googleapis.com"
        }.Build();

        // Setting the AnnotationKey and AnnotationValue
        AnnotationKey = "my-annotation-key";
        AnnotationValue = "my-annotation-value";

        // Setting the LabelKey and LabelValue
        LabelKey = "my-label-key";
        LabelValue = "my-label-value";

        // Required for testing regional samples
        Secret = CreateSecret(RandomId());
        SecretVersion = AddSecretVersion(Secret);
        SecretToCreateName = SecretName.FromProjectLocationSecret(ProjectId, LocationId, RandomId());
    }

    public void Dispose()
    {
        DeleteSecret(Secret.SecretName);
        DeleteSecret(SecretToCreateName);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Secret CreateSecret(string secretId)
    {
        LocationName locationName = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret
        {
            Labels =
          {
              { LabelKey, LabelValue }
          },
            Annotations = {
            { AnnotationKey, AnnotationValue }
          }
        };
        return client.CreateSecret(locationName, secretId, secret);
    }

    public SecretVersion AddSecretVersion(Secret secret)
    {
        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return client.AddSecretVersion(secret.SecretName, payload);
    }

    public void DeleteSecret(SecretName name)
    {
        try
        {
            client.DeleteSecret(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted.
            Console.Error.WriteLine($"Error deleting secret: {e.Message}");
        }
    }

    public SecretVersion DisableSecretVersion(SecretVersion version)
    {
        return client.DisableSecretVersion(version.SecretVersionName);
    }
}
