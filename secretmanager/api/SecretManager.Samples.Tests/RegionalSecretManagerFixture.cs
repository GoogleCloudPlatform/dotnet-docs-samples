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
using Google.Cloud.Iam.V1;
using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Text;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(RegionalSecretManagerFixture))]
public class RegionalSecretManagerFixture : IDisposable, ICollectionFixture<RegionalSecretManagerFixture>
{
    public SecretManagerServiceClient Client { get; }
    public string ProjectId { get; }
    public string LocationId { get; }
    public string AnnotationKey { get; }
    public string AnnotationValue { get; }
    public string LabelKey { get; }
    public string LabelValue { get; }
    public Secret Secret { get; }
    public SecretVersion SecretVersion { get; }
    public ProjectsClient ProjectsClient { get; }

    private const string CloudSqlRole = "roles/cloudsql.admin";

    // Bare ID of a pre-provisioned Cloud SQL instance in LocationId.
    public string CloudSqlInstanceId => RequiredEnvironmentVariable("CLOUD_SQL_INSTANCE");

    // Username of an existing database user on that instance.
    public string CloudSqlUsername => RequiredEnvironmentVariable("CLOUD_SQL_USER");
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
        Client = new SecretManagerServiceClientBuilder
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
        // Create the Projects client, used to grant Cloud SQL IAM permissions.
        ProjectsClient = ProjectsClient.Create();

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
        LocationName locationName = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret
        {
            Labels =
            {
                { LabelKey, LabelValue }
            },
            Annotations =
            {
              { AnnotationKey, AnnotationValue }
            }
        };
        return Client.CreateSecret(locationName, secretId, secret);
    }

    public Secret CreateSecretWithDelayedDestroy()
    {
        LocationName locationName = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret
        {
            VersionDestroyTtl = new Duration
            {
                Seconds = 24 * 60 * 60,
            }
        };
        return Client.CreateSecret(locationName, RandomId(), secret);
    }

    private static string RequiredEnvironmentVariable(string name)
    {
        string value = Environment.GetEnvironmentVariable(name);
        if (String.IsNullOrEmpty(value))
        {
            throw new Exception($"missing {name}");
        }
        return value;
    }

    /// <summary>
    /// Creates a Cloud SQL DB credentials secret and grants its own built-in identity
    /// Cloud SQL IAM permissions, which enabling managed rotation requires. The grant is
    /// per-secret, so callers must call RevokeCloudSqlRole once they are done.
    /// </summary>
    public Secret CreateCloudSqlCredentialsSecret(string secretId)
    {
        LocationName locationName = new LocationName(ProjectId, LocationId);
        Secret secret = Client.CreateSecret(locationName, secretId, new Secret
        {
            SecretType = Secret.Types.SecretType.CloudSqlDbCredentials,
        });

        try
        {
            if (secret.PolicyMember?.IamPolicyUidPrincipal == null)
            {
                throw new InvalidOperationException("The created regional secret does not have a valid PolicyMember or IamPolicyUidPrincipal.");
            }
            GrantCloudSqlRole(secret.PolicyMember.IamPolicyUidPrincipal);
        }
        catch
        {
            DeleteSecret(secret.SecretName);
            throw;
        }

        // IAM grants are eventually consistent; give it a moment before a caller
        // tries to use it for managed rotation.
        Thread.Sleep(TimeSpan.FromSeconds(10));
        return secret;
    }

    /// <summary>
    /// Grants the Cloud SQL role to the member on the project. SetIamPolicy replaces the
    /// whole policy, so this reads the current policy, adds the member, and writes it back
    /// with the same etag, retrying the read-modify-write if another writer raced us.
    /// </summary>
    public void GrantCloudSqlRole(string member)
    {
        UpdateProjectPolicy(policy =>
        {
            Binding binding = null;
            foreach (Binding candidate in policy.Bindings)
            {
                if (candidate.Role == CloudSqlRole && candidate.Condition == null)
                {
                    binding = candidate;
                    break;
                }
            }
            if (binding == null)
            {
                binding = new Binding { Role = CloudSqlRole };
                policy.Bindings.Add(binding);
            }
            if (binding.Members.Contains(member))
            {
                return false;
            }
            binding.Members.Add(member);
            return true;
        });
    }

    /// <summary>Removes the member from the Cloud SQL role, added by GrantCloudSqlRole.</summary>
    public void RevokeCloudSqlRole(string member)
    {
        UpdateProjectPolicy(policy =>
        {
            bool changed = false;
            foreach (Binding binding in policy.Bindings)
            {
                if (binding.Role == CloudSqlRole)
                {
                    changed |= binding.Members.Remove(member);
                }
            }
            return changed;
        });
    }

    private void UpdateProjectPolicy(Func<Policy, bool> modify)
    {
        ProjectName projectName = new ProjectName(ProjectId);
        for (int attempt = 1; ; attempt++)
        {
            Policy policy = ProjectsClient.GetIamPolicy(projectName.ToString());
            if (!modify(policy))
            {
                return;
            }
            try
            {
                ProjectsClient.SetIamPolicy(new SetIamPolicyRequest
                {
                    Resource = projectName.ToString(),
                    Policy = policy,
                });
                return;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Aborted && attempt < 5)
            {
                // Etag mismatch: another writer changed the policy; retry.
                Thread.Sleep(TimeSpan.FromSeconds(attempt));
            }
        }
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
            // Ignore error - secret was already deleted.
            Console.Error.WriteLine($"Error deleting secret: {e.Message}");
        }
    }

    public SecretVersion DisableSecretVersion(SecretVersion version)
    {
        return Client.DisableSecretVersion(version.SecretVersionName);
    }

    public SecretVersion DestroySecretVersion(SecretVersion version)
    {
        return Client.DestroySecretVersion(version.SecretVersionName);
    }
}
