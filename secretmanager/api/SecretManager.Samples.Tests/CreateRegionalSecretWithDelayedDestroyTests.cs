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

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;

using System;
using Xunit;


[Collection(nameof(RegionalSecretManagerFixture))]
public class CreateRegionalSecretWithDelayedDestroyTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly CreateRegionalSecretWithDelayedDestroySample _sample;

    public CreateRegionalSecretWithDelayedDestroyTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalSecretWithDelayedDestroySample();
    }

    [Fact]
    public void CreatesRegionalSecretsWithDelayedDestroy()
    {
        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = SecretName.FromProjectLocationSecret(_fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());

        // Set the timeToLive Value.
        int timeToLive = 86400;

        // Run the sample.
        Secret result = _sample.CreateRegionalSecretWithDelayedDestroy(
          projectId: secretName.ProjectId, locationId: secretName.LocationId, secretId: secretName.SecretId, timeToLive: timeToLive);

        // Assert that the secret was created with corect configurations
        Assert.Equal(result.SecretName.SecretId, secretName.SecretId);
        Assert.Equal(result.VersionDestroyTtl, Duration.FromTimeSpan(TimeSpan.FromSeconds(timeToLive)));

        // Clean the created secret.
        _fixture.DeleteSecret(secretName);
    }
}
