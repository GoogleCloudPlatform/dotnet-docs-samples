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

using Google.Cloud.SecretManager.V1;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class DestroySecretVersionTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DestroySecretVersionSample _sample;

    public DestroySecretVersionTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DestroySecretVersionSample();
    }

    [Fact]
    public void DestroysSecretVersions()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecret(_fixture.RandomId());

        // Add the secret version to the created secret.
        SecretVersion version = _fixture.AddSecretVersion(secret);

        // Get the secret version name.
        SecretVersionName secretVersionName = version.SecretVersionName;

        // Run the sample code.
        SecretVersion secretVersion = _sample.DestroySecretVersion(
          projectId: secretVersionName.ProjectId, secretId: secretVersionName.SecretId, secretVersionId: secretVersionName.SecretVersionId);

        // Assert that the Secret gets disabled.
        Assert.Equal(SecretVersion.Types.State.Destroyed, secretVersion.State);

        // Clean up the created resource
        _fixture.DeleteSecret(secret.SecretName);
    }

    [Fact]
    public void DisablesSecretVersionWhenDelayedDestroyEnabled()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecretWithDelayedDestroy();

        // Add the secret version to the created secret.
        SecretVersion version = _fixture.AddSecretVersion(secret);

        // Run the sample code.
        SecretVersion secretVersion = _sample.DestroySecretVersion(
          projectId: secret.SecretName.ProjectId, secretId: secret.SecretName.SecretId, secretVersionId: version.SecretVersionName.SecretVersionId);

        // Assert that the Secret gets disabled.
        Assert.Equal(SecretVersion.Types.State.Disabled, secretVersion.State);

        // Clean up the created resource
        _fixture.DeleteSecret(secret.SecretName);
    }
}
