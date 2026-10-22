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
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class RotateRegionalSecretTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EnableRegionalSecretManagedRotationSample _enableSample;
    private readonly RotateRegionalSecretSample _sample;

    public RotateRegionalSecretTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _enableSample = new EnableRegionalSecretManagedRotationSample();
        _sample = new RotateRegionalSecretSample();
    }

    [Fact]
    public void RotatesRegionalSecret()
    {
        Secret secret = null;
        string member = null;
        try
        {
            secret = _fixture.CreateCloudSqlCredentialsSecret(_fixture.RandomId());
            member = secret.PolicyMember.IamPolicyUidPrincipal;

            SecretVersion firstVersion = _enableSample.EnableRegionalSecretManagedRotation(
              secret.SecretName.ProjectId, secret.SecretName.LocationId, secret.SecretName.SecretId,
              _fixture.CloudSqlInstanceId, _fixture.CloudSqlUsername);

            // Run the sample code.
            SecretVersion rotatedVersion = _sample.RotateRegionalSecret(
              projectId: secret.SecretName.ProjectId,
              locationId: secret.SecretName.LocationId,
              secretId: secret.SecretName.SecretId);

            // Assert that rotation created a new, enabled version.
            Assert.Contains(secret.SecretName.SecretId, rotatedVersion.Name);
            Assert.NotEqual(firstVersion.Name, rotatedVersion.Name);
            Assert.Equal(SecretVersion.Types.State.Enabled, rotatedVersion.State);
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
