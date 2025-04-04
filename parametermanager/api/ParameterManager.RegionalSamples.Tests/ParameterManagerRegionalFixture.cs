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
    public string SecretReference { get; }
    public string ParameterId { get; }
    public string ParameterVersionId { get; }

    public Parameter ParameterToRender { get; }
    public ParameterVersion ParameterVersionToRender { get; }
    public ParameterVersionName ParameterVersionNameToRender { get; }
    public Secret Secret { get; }
    public SecretVersion SecretVersion { get; }
    public Policy Policy { get; }

    public ParameterManagerRegionalFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        ParameterId = RandomId();
        ParameterVersionId = RandomId();
        ParameterToRender = CreateParameter(ParameterId, ParameterFormat.Json);
        Secret = CreateSecret(RandomId());
        SecretVersion = AddSecretVersion(Secret);
        SecretReference = $"{{\"username\": \"test-user\", \"password\": \"__REF__(//secretmanager.googleapis.com/{Secret.SecretName}/versions/latest)\"}}";
        ParameterVersionToRender = CreateParameterVersion(ParameterId, ParameterVersionId, SecretReference);
        Policy = GrantIAMAccess(Secret.SecretName, ParameterToRender.PolicyMember.IamPolicyUidPrincipal);
        ParameterVersionNameToRender = new ParameterVersionName(ProjectId, LocationId, ParameterId, ParameterVersionId);
        Thread.Sleep(120000);
    }

    public void Dispose()
    {
        DeleteParameterVersion(ParameterVersionNameToRender);
        DeleteParameter(ParameterToRender.ParameterName);
        DeleteSecret(Secret.SecretName);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Parameter CreateParameter(string parameterId, ParameterFormat format)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        LocationName parent = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format
        };

        return client.CreateParameter(parent, parameter, parameterId);
    }

    public ParameterVersion CreateParameterVersion(string parameterId, string versionId, string payload)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        ParameterName parameterName = new ParameterName(ProjectId, LocationId, parameterId);
        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload, Encoding.UTF8)
            }
        };

        return client.CreateParameterVersion(parameterName, parameterVersion, versionId);
    }

    private void DeleteParameter(ParameterName name)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();
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
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();
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
        string regionalEndpoint = $"secretmanager.{LocationId}.rep.googleapis.com";
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        LocationName parent = new LocationName(ProjectId, LocationId);

        Secret secret = new Secret();

        return client.CreateSecret(parent, secretId, secret);
    }

    public Policy GrantIAMAccess(SecretName secretName, string member)
    {
        string regionalEndpoint = $"secretmanager.{LocationId}.rep.googleapis.com";
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Get current policy.
        Policy policy = client.GetIamPolicy(new GetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
        });

        // Add the user to the list of bindings.
        policy.AddRoleMember("roles/secretmanager.secretAccessor", member);

        // Save the updated policy.
        policy = client.SetIamPolicy(new SetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
            Policy = policy,
        });
        return policy;
    }
    private SecretVersion AddSecretVersion(Secret secret)
    {
        string regionalEndpoint = $"secretmanager.{LocationId}.rep.googleapis.com";
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        return client.AddSecretVersion(secret.SecretName, payload);
    }

    private void DeleteSecret(SecretName name)
    {
        string regionalEndpoint = $"secretmanager.{LocationId}.rep.googleapis.com";
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = regionalEndpoint
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
}
