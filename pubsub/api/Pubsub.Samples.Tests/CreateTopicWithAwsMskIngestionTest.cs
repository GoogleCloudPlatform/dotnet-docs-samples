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
public class CreateTopicWithAwsMskIngestionTest
{
  private readonly PubsubFixture _pubsubFixture;
  private readonly CreateTopicWithAwsMskIngestionSample _createTopicWithAwsMskIngestionSample;

  public CreateTopicWithAwsMskIngestionTest(PubsubFixture pubsubFixture)
  {
    _pubsubFixture = pubsubFixture;
    _createTopicWithAwsMskIngestionSample = new CreateTopicWithAwsMskIngestionSample();
  }

  [Fact]
  public void CreateTopicWithAwsMskIngestion()
  {
    string topicId = _pubsubFixture.RandomTopicId();
    var (clusterArn, mskTopic, awsRoleArn, gcpServiceAccount) = _pubsubFixture.RandomAwsMskIngestionParams();
    Topic createdTopic = _createTopicWithAwsMskIngestionSample.CreateTopicWithAwsMskIngestion(_pubsubFixture.ProjectId, topicId, clusterArn, mskTopic, awsRoleArn, gcpServiceAccount);

    // Confirm that the created topic and topic retrieved by ID are equal
    Topic retrievedTopic = _pubsubFixture.GetTopic(topicId);
    Assert.Equal(createdTopic, retrievedTopic);

    // Confirm that all Amazon MSK Ingestion params are equal to expected values
    Assert.Equal(clusterArn, createdTopic.IngestionDataSourceSettings.AwsMsk.ClusterArn);
    Assert.Equal(mskTopic, createdTopic.IngestionDataSourceSettings.AwsMsk.Topic);
    Assert.Equal(awsRoleArn, createdTopic.IngestionDataSourceSettings.AwsMsk.AwsRoleArn);
    Assert.Equal(gcpServiceAccount, createdTopic.IngestionDataSourceSettings.AwsMsk.GcpServiceAccount);
  }
}