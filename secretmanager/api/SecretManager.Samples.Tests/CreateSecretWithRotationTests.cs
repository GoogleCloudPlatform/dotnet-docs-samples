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

using Google.Cloud.Iam.V1;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SecretManagerFixture))]
public class CreateSecretWithRotationTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithRotationSample _sample;

    public CreateSecretWithRotationTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithRotationSample();
    }

    [Fact]
    public void CreatesSecretWithRotation()
    {
        // Get the SecretName to create Secret
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        // Create the secret with rotation configuration
        Secret result = _sample.CreateSecretWithRotation(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId,
            topicName: _fixture.TopicName);

        // Verify rotation configuration
        Assert.NotNull(result.Rotation);
        Assert.NotNull(result.Rotation.NextRotationTime);
        Assert.NotNull(result.Rotation.RotationPeriod);
        Assert.Equal(86400, result.Rotation.RotationPeriod.Seconds);

        // Verify topic configuration
        Assert.NotEmpty(result.Topics);
        Assert.Contains(result.Topics, topic => topic.Name == _fixture.TopicName);

        // Verify next rotation time is in the future (approximately 24 hours from now)
        DateTime nextRotation = result.Rotation.NextRotationTime.ToDateTime();
        DateTime now = DateTime.UtcNow;
        TimeSpan difference = nextRotation - now;

        double differenceInMinutes = difference.TotalMinutes;
        Assert.True(differenceInMinutes > 1438 && differenceInMinutes < 1442,
            $"Next rotation time should be approximately 24 hours (1440 minutes) from now, but was {differenceInMinutes} minutes");

        _fixture.DeleteSecret(secretName);

    }
};
