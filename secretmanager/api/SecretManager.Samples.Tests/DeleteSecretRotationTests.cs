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

[Collection(nameof(SecretManagerFixture))]
public class DeleteSecretRotationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly DeleteSecretRotationSample _deleteSample;

    public DeleteSecretRotationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _deleteSample = new DeleteSecretRotationSample();
    }

    [Fact]
    public void DeletesSecretRotation()
    {
        // First create a secret with rotation (24 hours by default)
        Secret createdSecret = _fixture.CreateSecretWithRotation();

        // Verify initial rotation configuration exists
        Assert.NotNull(createdSecret.Rotation);
        Assert.NotNull(createdSecret.Rotation.RotationPeriod);
        Assert.NotNull(createdSecret.Rotation.NextRotationTime);

        // Delete the rotation configuration
        Secret updatedSecret = _deleteSample.DeleteSecretRotation(
            projectId: createdSecret.SecretName.ProjectId,
            secretId: createdSecret.SecretName.SecretId);

        // Verify the rotation configuration was removed
        Assert.Null(updatedSecret.Rotation);

        // Verify the topic configuration was preserved
        Assert.NotEmpty(updatedSecret.Topics);
        Assert.Contains(updatedSecret.Topics, topic => topic.Name == _fixture.TopicName);

        _fixture.DeleteSecret(createdSecret.SecretName);
    }
}
