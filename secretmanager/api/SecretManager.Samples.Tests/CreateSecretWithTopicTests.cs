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
public class CreateSecretWithTopicTests
{
    private readonly SecretManagerFixture _fixture;
    private readonly CreateSecretWithTopicSample _sample;

    public CreateSecretWithTopicTests(SecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateSecretWithTopicSample();
    }

    [Fact]
    public void CreatesSecretWithTopic()
    {
        // Get the SecretName to create Secret
        SecretName secretName = new SecretName(_fixture.ProjectId, _fixture.RandomId());

        // Create the secret with topic configuration
        Secret result = _sample.CreateSecretWithTopic(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId,
            topicName: _fixture.TopicName);

        // Verify topic configuration
        Assert.NotEmpty(result.Topics);
        Assert.Single(result.Topics);
        Assert.Equal(_fixture.TopicName, result.Topics[0].Name);

        _fixture.DeleteSecret(secretName);
    }
}
