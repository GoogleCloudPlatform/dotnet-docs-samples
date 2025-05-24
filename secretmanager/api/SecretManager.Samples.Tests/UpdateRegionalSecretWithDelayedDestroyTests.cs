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
using Google.Protobuf.WellKnownTypes;
using System;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class UpdateRegionalSecretWithDelayedDestroyTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly UpdateRegionalSecretWithDelayedDestroySample _sample;

    public UpdateRegionalSecretWithDelayedDestroyTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateRegionalSecretWithDelayedDestroySample();
    }

    [Fact]
    public void UpdatesRegionalSecretsWithDelayedDestroy()
    {
        // Create the secret and add secret version.
        Secret secret = _fixture.CreateSecretWithDelayedDestroy();

        // Set the updated time to live value.
        int updatedTimeToLive = 172800;

        // Run the sample.
        Secret result = _sample.UpdateRegionalSecretWithDelayedDestroy(
          projectId: secret.SecretName.ProjectId,
          locationId: secret.SecretName.LocationId,
          secretId: secret.SecretName.SecretId,
          updatedTimeToLive: updatedTimeToLive
        );

        // Assert that the secret was updated with the correct configurations.
        Assert.Equal(result.SecretName.SecretId, secret.SecretName.SecretId);
        Assert.Equal(result.VersionDestroyTtl, Duration.FromTimeSpan(TimeSpan.FromSeconds(updatedTimeToLive)));

        // Clean the created secret.
        _fixture.DeleteSecret(secret.SecretName);
    }
}
