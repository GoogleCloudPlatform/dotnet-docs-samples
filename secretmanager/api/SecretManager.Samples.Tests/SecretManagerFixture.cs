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
using Google.Protobuf.WellKnownTypes;
using System;
using System.Text;
using Xunit;


[CollectionDefinition(nameof(SecretManagerFixture))]
public class SecretManagerFixture : IDisposable, ICollectionFixture<SecretManagerFixture>
{
    public SecretManagerServiceClient Client { get; }
    public string ProjectId { get; }
    public ProjectName ProjectName { get; }
    public Secret Secret { get; }
    public SecretVersion SecretVersion { get; }

    public string AnnotationKey { get; }
    public string AnnotationValue { get; }
    public string LabelKey { get; }
    public string LabelValue { get; }

    public SecretManagerFixture()
    {
        // Get the Google Cloud ProjectId.
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Get the ProjectName from ProjectId,
        ProjectName = new ProjectName(ProjectId);

        // Create the Regional Secret Manager Client
        Client = SecretManagerServiceClient.Create();

        // Setting the AnnotationKey and AnnotationValue
        AnnotationKey = "my-annotation-key";
        AnnotationValue = "my-annotation-value";

        // Setting the LabelKey and LabelValue
        LabelKey = "my-label-key";
        LabelValue = "my-label-value";

        Secret = CreateSecret(RandomId());
        SecretVersion = AddSecretVersion(Secret);
    }

    public void Dispose()
    {
        DeleteSecret(Secret.SecretName);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Secret CreateSecret(string secretId)
    {
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
            Labels =
            {
                { LabelKey, LabelValue }
            },
            Annotations =
            {
              { AnnotationKey, AnnotationValue }
            }
        };

        return Client.CreateSecret(ProjectName, secretId, secret);
    }

    public Secret CreateSecretWithDelayedDestroy()
    {
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
            VersionDestroyTtl = new Duration
            {
                Seconds = 24 * 60 * 60,
            }

        };
        return Client.CreateSecret(ProjectName, RandomId(), secret);
    }

    public SecretVersion AddSecretVersion(Secret secret)
    {
        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return Client.AddSecretVersion(secret.SecretName, payload);
    }

    public void DeleteSecret(SecretName name)
    {
        try
        {
            Client.DeleteSecret(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted
        }
    }

    public SecretVersion DestroySecretVersion(SecretVersion version)
    {
        return Client.DestroySecretVersion(version.SecretVersionName);
    }

    public void DisableSecretVersion(SecretVersion version)
    {
        Client.DisableSecretVersion(version.SecretVersionName);
    }
}
