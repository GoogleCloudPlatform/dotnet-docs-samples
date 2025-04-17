// Copyright(c) 2025 Google Inc.
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
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.SecretManager.V1;
using Google.Cloud.Iam.V1;
using Google.Protobuf;
using System.Text;

[CollectionDefinition(nameof(ParameterManagerFixture))]
public class ParameterManagerFixture : IDisposable, ICollectionFixture<ParameterManagerFixture>
{
    public string ProjectId { get; }
    public const string LocationId = "global";

    public ParameterManagerClient client { get; }
    public SecretManagerServiceClient secretClient { get; }
    internal List<ParameterName> ParametersToDelete { get; } = new List<ParameterName>();
    internal List<SecretName> SecretsToDelete { get; } = new List<SecretName>();
    internal List<ParameterVersionName> ParameterVersionsToDelete { get; } = new List<ParameterVersionName>();

    public ParameterManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        client = ParameterManagerClient.Create();
        secretClient = SecretManagerServiceClient.Create();
    }

    public void Dispose()
    {
        foreach (var parameterVersion in ParameterVersionsToDelete)
        {
            DeleteParameterVersion(parameterVersion);
        }
        foreach (var parameter in ParametersToDelete)
        {
            DeleteParameter(parameter);
        }
        foreach (var secret in SecretsToDelete)
        {
            DeleteSecret(secret);
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Parameter CreateParameter(string parameterId, ParameterFormat format)
    {
        LocationName projectName = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format
        };

        Parameter Parameter = client.CreateParameter(projectName, parameter, parameterId);
        ParametersToDelete.Add(Parameter.ParameterName);
        return Parameter;
    }

    public ParameterVersion CreateParameterVersion(string parameterId, string versionId, string payload)
    {
        ParameterName parameterName = new ParameterName(ProjectId, LocationId, parameterId);
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload, Encoding.UTF8)
            }
        };

        ParameterVersion ParameterVersion = client.CreateParameterVersion(parameterName, parameterVersion, versionId);
        ParameterVersionsToDelete.Add(ParameterVersion.ParameterVersionName);
        return ParameterVersion;
    }

    private void DeleteParameter(ParameterName name)
    {
        try
        {
            client.DeleteParameter(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }

    private void DeleteParameterVersion(ParameterVersionName name)
    {
        try
        {
            client.DeleteParameterVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter version was already deleted
        }
    }


    public Secret CreateSecret(string secretId)
    {
        ProjectName projectName = new ProjectName(ProjectId);

        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
        };
        Secret Secret = secretClient.CreateSecret(projectName, secretId, secret);
        SecretsToDelete.Add(Secret.SecretName);
        return Secret;
    }

    public Policy GrantIAMAccess(SecretName secretName, string member)
    {
        // Get current policy.
        Policy policy = secretClient.GetIamPolicy(new GetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
        });

        // Add the user to the list of bindings.
        policy.AddRoleMember("roles/secretmanager.secretAccessor", member);

        // Save the updated policy.
        policy = secretClient.SetIamPolicy(new SetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
            Policy = policy,
        });
        return policy;
    }

    public SecretVersion AddSecretVersion(Secret secret)
    {
        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return secretClient.AddSecretVersion(secret.SecretName, payload);
    }

    private void DeleteSecret(SecretName name)
    {
        try
        {
            secretClient.DeleteSecret(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted
        }
    }
}
