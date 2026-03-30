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
using System.Linq;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class CreateRegionalSecretWithTopicTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly CreateRegionalSecretWithTopicSample _sample;

    public CreateRegionalSecretWithTopicTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalSecretWithTopicSample();
    }

    [Fact]
    public void CreatesRegionalSecretWithTopic()
    {
        // Get the SecretName from the set ProjectId & LocationId.
        SecretName secretName = SecretName.FromProjectLocationSecret(
            _fixture.ProjectId, _fixture.LocationId, _fixture.RandomId());
        // Create a regional secret with topic notification
        Secret createdSecret = _sample.CreateRegionalSecretWithTopic(
            projectId: secretName.ProjectId,
            secretId: secretName.SecretId,
            locationId: secretName.LocationId,
            topicName: _fixture.TopicName);

        // Verify the topic was configured
        Assert.NotEmpty(createdSecret.Topics);
        Assert.Contains(createdSecret.Topics, topic => topic.Name == _fixture.TopicName);

        _fixture.DeleteSecret(secretName);
    }
}
