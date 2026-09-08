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
using Google.Protobuf.WellKnownTypes;
using System;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class DeleteRegionalSecretRotationTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly DeleteRegionalSecretRotationSample _deleteSample;

    public DeleteRegionalSecretRotationTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _deleteSample = new DeleteRegionalSecretRotationSample();
    }

    [Fact]
    public void DeletesRegionalSecretRotation()
    {
        // First create a regional secret with rotation
        Secret createdSecret = _fixture.CreateSecretWithRotation();

        // Verify the secret has rotation configuration
        Assert.NotNull(createdSecret.Rotation);
        Assert.NotEqual(0, createdSecret.Rotation.RotationPeriod.Seconds);
        Assert.NotNull(createdSecret.Rotation.NextRotationTime);

        // Delete rotation configuration using our sample
        Secret updatedSecret = _deleteSample.DeleteRegionalSecretRotation(
            projectId: createdSecret.SecretName.ProjectId,
            secretId: createdSecret.SecretName.SecretId,
            locationId: createdSecret.SecretName.LocationId);

        // Verify rotation configuration was removed
        // The rotation field should either be null or have default values
        if (updatedSecret.Rotation != null)
        {
            // If not null, verify it has default/empty values
            Assert.Equal(0, updatedSecret.Rotation.RotationPeriod.Seconds);
            Assert.Equal(0, updatedSecret.Rotation.NextRotationTime.Seconds);
        }

        // Clean up
        _fixture.DeleteSecret(createdSecret.SecretName);
    }
}
