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

[Collection(nameof(SecretManagerFixture))]
public class UpdateSecretRotationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly UpdateSecretRotationSample _updateSample;

    public UpdateSecretRotationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _updateSample = new UpdateSecretRotationSample();
    }

    [Fact]
    public void UpdatesSecretRotation()
    {
        // First create a secret with rotation (24 hours by default)
        Secret createdSecret = _fixture.CreateSecretWithRotation();

        // Verify initial rotation period is 24 hours (86400 seconds)
        Assert.Equal(86400, createdSecret.Rotation.RotationPeriod.Seconds);

        // Update the rotation period to 48 hours
        Secret updatedSecret = _updateSample.UpdateSecretRotation(
            projectId: createdSecret.SecretName.ProjectId,
            secretId: createdSecret.SecretName.SecretId);

        // Verify the rotation period was updated to 48 hours (172800 seconds)
        Assert.Equal(172800, updatedSecret.Rotation.RotationPeriod.Seconds);

        // Verify the next rotation time was not changed (should be the same as before)
        Assert.Equal(
            createdSecret.Rotation.NextRotationTime.Seconds,
            updatedSecret.Rotation.NextRotationTime.Seconds);

        // Verify the topic configuration was preserved
        Assert.NotEmpty(updatedSecret.Topics);
        Assert.Contains(updatedSecret.Topics, topic => topic.Name == _fixture.TopicName);

        _fixture.DeleteSecret(createdSecret.SecretName);

    }
}
