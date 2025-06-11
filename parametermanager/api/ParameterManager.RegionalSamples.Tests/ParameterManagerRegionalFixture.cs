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
using Google.Cloud.Iam.V1;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using System.Text;

[CollectionDefinition(nameof(ParameterManagerRegionalFixture))]
public class ParameterManagerRegionalFixture : IDisposable, ICollectionFixture<ParameterManagerRegionalFixture>
{
    public string ProjectId { get; }
    public const string LocationId = "us-central1";

    public ParameterManagerClient Client { get; }
    public SecretManagerServiceClient SecretClient { get; }
    internal List<ParameterName> ParametersToDelete { get; } = new List<ParameterName>();
    internal List<SecretName> SecretsToDelete { get; } = new List<SecretName>();
    internal List<ParameterVersionName> ParameterVersionsToDelete { get; } = new List<ParameterVersionName>();

    public ParameterManagerRegionalFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        Client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        string regionalSecretEndpoint = $"secretmanager.{LocationId}.rep.googleapis.com";
        SecretClient = new SecretManagerServiceClientBuilder
        {
            Endpoint = regionalSecretEndpoint
        }.Build();
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

    public (string parameterId, string versionId, Parameter parameter, string payload) CreateParameterWithVersion()
    {
        string parameterId = RandomId();
        string versionId = RandomId();
        Parameter parameter = CreateParameter(parameterId, ParameterFormat.Unformatted);
        string payload = "test123";
        return (parameterId, versionId, parameter, payload);
    }

    public Parameter CreateParameter(string parameterId, ParameterFormat format)
    {
        LocationName parent = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format
        };

        Parameter Parameter = Client.CreateParameter(parent, parameter, parameterId);
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

        ParameterVersion ParameterVersion = Client.CreateParameterVersion(parameterName, parameterVersion, versionId);
        ParameterVersionsToDelete.Add(ParameterVersion.ParameterVersionName);
        return ParameterVersion;
    }

    private void DeleteParameter(ParameterName name)
    {
        try
        {
            Client.DeleteParameter(name);
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
            Client.DeleteParameterVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter version was already deleted
        }
    }

    public Secret CreateSecret(string secretId)
    {
        LocationName parent = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret();

        Secret Secret = SecretClient.CreateSecret(parent, secretId, secret);
        SecretsToDelete.Add(Secret.SecretName);
        return Secret;
    }

    public Policy GrantIAMAccess(SecretName secretName, string member)
    {
        // Get current policy.
        Policy policy = SecretClient.GetIamPolicy(new GetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
        });

        // Add the user to the list of bindings.
        policy.AddRoleMember("roles/secretmanager.secretAccessor", member);

        // Save the updated policy.
        policy = SecretClient.SetIamPolicy(new SetIamPolicyRequest
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

        return SecretClient.AddSecretVersion(secret.SecretName, payload);
    }

    private void DeleteSecret(SecretName name)
    {
        try
        {
            SecretClient.DeleteSecret(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - secret was already deleted
        }
    }
}
