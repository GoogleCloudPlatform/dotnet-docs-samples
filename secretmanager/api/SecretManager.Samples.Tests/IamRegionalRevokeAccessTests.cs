/*
 * Copyright 2024 Google LLC
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

using Google.Cloud.Iam.V1;
using Google.Cloud.SecretManager.V1;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class IamRegionalRevokeAccessTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly IamRegionalRevokeAccessSample _sample;

    public IamRegionalRevokeAccessTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new IamRegionalRevokeAccessSample();
    }

    [Fact]
    public void GrantsAccess()
    {
        string member = "group:test@google.com";
        // Get the secret name.
        SecretName secretName = _fixture.Secret.SecretName;

        // Update the policy to add the user to the binding's list.
        Policy policy = _fixture.Client.GetIamPolicy(new GetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
        });
        policy.AddRoleMember("roles/secretmanager.secretAccessor", member);
        policy = _fixture.Client.SetIamPolicy(new SetIamPolicyRequest
        {
            ResourceAsResourceName = secretName,
            Policy = policy,
        });

        // Run the code sample.
        Policy result = _sample.IamRegionalRevokeAccess(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId,
          member: member);

        // Assert that the binding does not contain the expected role and members.
        Assert.DoesNotContain(result.Bindings,
            binding => binding.Role == "roles/secretmanager.secretAccessor" && binding.Members.Contains("group:test@google.com"));
    }
}
