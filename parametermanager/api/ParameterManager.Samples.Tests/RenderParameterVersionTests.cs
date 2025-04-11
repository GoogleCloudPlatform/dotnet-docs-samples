/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.SecretManager.V1;
using Google.Cloud.Iam.V1;
using Google.Protobuf;
using System.Text;


[Collection(nameof(ParameterManagerFixture))]
public class RenderParameterVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly RenderParameterVersionSample _sample;

    public RenderParameterVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new RenderParameterVersionSample();
    }

    public Secret CreateSecret(string secretId)
    {
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        ProjectName projectName = new ProjectName(_fixture.ProjectId);

        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
        };

        return client.CreateSecret(projectName, secretId, secret);
    }

    public Policy GrantIAMAccess(SecretName secretName, string member)
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

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

    [Fact]
    public void RenderParameterVersion()
    {
        string parameterId = _fixture.RandomId();
        string versionId = _fixture.RandomId();
        Parameter parameter = _fixture.CreateParameter(parameterId, ParameterFormat.Unformatted);

        Secret secret = CreateSecret(_fixture.RandomId());
        AddSecretVersion(secret);
        string payload = $"{{\"username\": \"test-user\", \"password\": \"__REF__(//secretmanager.googleapis.com/{secret.SecretName}/versions/latest)\"}}";
        GrantIAMAccess(secret.SecretName, parameter.PolicyMember.IamPolicyUidPrincipal);

        ParameterVersion parameterVersion = _fixture.CreateParameterVersion(parameterId, versionId, payload);

        string result = _sample.RenderParameterVersion(projectId: _fixture.ProjectId, parameterId: parameterId, versionId);

        Assert.NotNull(result);

        _fixture.ParametersToDelete.Add(parameter.ParameterName);
        _fixture.ParameterVersionsToDelete.Add(parameterVersion.ParameterVersionName);
        DeleteSecret(secret.SecretName);
    }
}
