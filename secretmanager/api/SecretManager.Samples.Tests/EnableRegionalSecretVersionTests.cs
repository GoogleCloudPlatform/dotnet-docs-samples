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

using Google.Cloud.SecretManager.V1;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class EnableRegionalSecretVersionTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EnableRegionalSecretVersionSample _sample;

    public EnableRegionalSecretVersionTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EnableRegionalSecretVersionSample();
    }

    [Fact]
    public void EnablesRegionalSecretVersions()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());
        SecretVersion addSecretVersion = _fixture.AddSecretVersion(secret);
        SecretVersionName secretVersionName = addSecretVersion.SecretVersionName;

        // Run the code sample.
        SecretVersion secretVersion = _sample.EnableRegionalSecretVersion(
          projectId: secretVersionName.ProjectId,
          locationId: secretVersionName.LocationId,
          secretId: secretVersionName.SecretId,
          secretVersionId: secretVersionName.SecretVersionId
        );

        // Assert that the secret version is in enabled state.
        Assert.Equal(SecretVersion.Types.State.Enabled, secretVersion.State);

        // Clean the created secret.
        _fixture.DeleteSecret(secret.SecretName);
    }

    [Fact]
    public void EnablesSecretVersionScheduledForDestruction()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecretWithDelayedDestroy();

        // Add the secret version to the created secret.
        SecretVersion version = _fixture.AddSecretVersion(secret);
        SecretVersionName secretVersionName = version.SecretVersionName;

        // Destroy the created secret.
        SecretVersion destroyedVersion = _fixture.DestroySecretVersion(version);

        // Run the code sample.
        SecretVersion secretVersion = _sample.EnableRegionalSecretVersion(
          projectId: secretVersionName.ProjectId,
          locationId: secretVersionName.LocationId,
          secretId: secretVersionName.SecretId,
          secretVersionId: secretVersionName.SecretVersionId
        );

        // Assert that the Secret gets disabled.
        Assert.Equal(SecretVersion.Types.State.Enabled, secretVersion.State);

        // Clean up the created resource
        _fixture.DeleteSecret(secret.SecretName);
    }
}
