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
public class CreateTopicWithConfluentCloudIngestionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithConfluentCloudIngestionSample _createTopicWithConfluentCloudIngestionSample;

    public CreateTopicWithConfluentCloudIngestionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithConfluentCloudIngestionSample = new CreateTopicWithConfluentCloudIngestionSample();
    }

    [Fact]
    public void CreateTopicWithConfluentCloudIngestion()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        var (bootstrapServer, clusterId, confluentTopic, identityPoolId, gcpServiceAccount) = _pubsubFixture.RandomConfluentCloudIngestionParams();
        Topic createdTopic = _createTopicWithConfluentCloudIngestionSample.CreateTopicWithConfluentCloudIngestion(_pubsubFixture.ProjectId, topicId, bootstrapServer, clusterId, confluentTopic, identityPoolId, gcpServiceAccount);

        // Confirm that the created topic and topic retrieved by ID are equal
        Topic retrievedTopic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(createdTopic, retrievedTopic);

        // Confirm that all Confluent Cloud Ingestion params are equal to expected values
        Assert.Equal(bootstrapServer, createdTopic.IngestionDataSourceSettings.ConfluentCloud.BootstrapServer);
        Assert.Equal(clusterId, createdTopic.IngestionDataSourceSettings.ConfluentCloud.ClusterId);
        Assert.Equal(confluentTopic, createdTopic.IngestionDataSourceSettings.ConfluentCloud.Topic);
        Assert.Equal(identityPoolId, createdTopic.IngestionDataSourceSettings.ConfluentCloud.IdentityPoolId);
        Assert.Equal(gcpServiceAccount, createdTopic.IngestionDataSourceSettings.ConfluentCloud.GcpServiceAccount);
    }
}
