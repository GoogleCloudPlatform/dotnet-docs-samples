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
using System.IO;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class UpdateRegionalSecretExpirationTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly UpdateRegionalSecretExpirationSample _updateSample;

    public UpdateRegionalSecretExpirationTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _updateSample = new UpdateRegionalSecretExpirationSample();
    }

    [Fact]
    public void UpdatesRegionalSecretExpiration()
    {

        // First, create a secret with initial expiration time (1 hour)
        Secret initialSecret = _fixture.CreateSecretWithExpireTime();
        try
        {
            // Update the secret to have a 2-hour expiration
            Secret result = _updateSample.UpdateRegionalSecretExpiration(
                projectId: initialSecret.SecretName.ProjectId,
                secretId: initialSecret.SecretName.SecretId,
                locationId: initialSecret.SecretName.LocationId);

            DateTime expectedTime = DateTime.UtcNow.AddHours(2);
            DateTime actualTime = result.ExpireTime.ToDateTime();

            // Allow for a small time difference (e.g., 30 seconds) due to test execution
            Assert.True(
                (actualTime - expectedTime).Duration() < TimeSpan.FromSeconds(60),
                $"Expected expiration time around {expectedTime}, but got {actualTime}"
            );
        }
        finally
        {
            // Clean the created secret
            _fixture.DeleteSecret(initialSecret.SecretName);
        }
    }
}
