/*
 * Copyright 2020 Google LLC
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
public class IamRegionalGrantAccessTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly IamRegionalGrantAccessSample _sample;

    public IamRegionalGrantAccessTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new IamRegionalGrantAccessSample();
    }

    [Fact]
    public void GrantsAccess()
    {
        SecretName secretName = _fixture.SecretToCreateName;
        Policy result = _sample.IamRegionalGrantAccess(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId,
          member: "group:test@google.com");
        Assert.Contains(result.Bindings,
            binding => binding.Role == "roles/secretmanager.secretAccessor" && binding.Members.Contains("group:test@google.com"));
    }
}
