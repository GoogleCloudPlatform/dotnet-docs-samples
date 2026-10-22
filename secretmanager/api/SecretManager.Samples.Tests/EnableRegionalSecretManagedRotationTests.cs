/*
 * Copyright 2026 Google LLC
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

using Google.Cloud.SecretManager.V1;
using System;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class EnableRegionalSecretManagedRotationTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EnableRegionalSecretManagedRotationSample _sample;

    public EnableRegionalSecretManagedRotationTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EnableRegionalSecretManagedRotationSample();
    }

    [Fact]
    public void EnablesRegionalSecretManagedRotation()
    {
        Secret secret = null;
        string member = null;
        try
        {
            secret = _fixture.CreateCloudSqlCredentialsSecret(_fixture.RandomId());
            member = secret.PolicyMember.IamPolicyUidPrincipal;

            // Run the sample code.
            SecretVersion version = _sample.EnableRegionalSecretManagedRotation(
              projectId: secret.SecretName.ProjectId,
              locationId: secret.SecretName.LocationId,
              secretId: secret.SecretName.SecretId,
              instanceId: _fixture.CloudSqlInstanceId,
              username: _fixture.CloudSqlUsername);

            // Assert that the first version was created and is enabled.
            Assert.Contains(secret.SecretName.SecretId, version.Name);
            Assert.Equal(SecretVersion.Types.State.Enabled, version.State);
        }
        finally
        {
            if (secret != null)
            {
                _fixture.DeleteSecret(secret.SecretName);
            }
            if (member != null)
            {
                _fixture.RevokeCloudSqlRole(member);
            }
        }
    }
}
