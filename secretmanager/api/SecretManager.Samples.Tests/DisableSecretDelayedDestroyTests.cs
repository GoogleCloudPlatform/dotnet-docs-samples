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
public class DisableSecretDelayedDestroyTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DisableSecretDelayedDestroySample _sample;

    public DisableSecretDelayedDestroyTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DisableSecretDelayedDestroySample();
    }

    [Fact]
    public void DisablesSecretDelayedDestroy()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecretWithDelayedDestroy();

        // Run the sample code.
        Secret disabledSecret = _sample.DisableSecretDelayedDestroy(
          projectId: secret.SecretName.ProjectId, secretId: secret.SecretName.SecretId);

        // Assert that the secret was created with the correct configurations.
        Assert.Null(disabledSecret.VersionDestroyTtl);

        // Clean the created secret.
        _fixture.DeleteSecret(secret.SecretName);
    }
}
