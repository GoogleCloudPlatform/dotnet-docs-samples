// Copyright 2025 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.PubSub.V1;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class UpdateTopicTypeTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly UpdateTopicTypeSample _updateTopicTypeSample;

    public UpdateTopicTypeTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _updateTopicTypeSample = new UpdateTopicTypeSample();
    }

    [Fact]
    public void UpdateTopicType()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        _pubsubFixture.CreateTopic(topicId);

        var (streamArn, consumerArn, awsRoleArn, gcpServiceAccount) = _pubsubFixture.RandomKinesisIngestionParams();
        Topic updatedTopic = _updateTopicTypeSample.UpdateTopicType(_pubsubFixture.ProjectId, topicId, streamArn, consumerArn, awsRoleArn, gcpServiceAccount);
        Topic retrievedTopic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(updatedTopic, retrievedTopic);
    }
}
