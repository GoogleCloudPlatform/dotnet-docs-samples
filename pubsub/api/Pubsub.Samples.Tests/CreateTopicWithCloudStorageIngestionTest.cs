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
using Google.Protobuf.WellKnownTypes;
using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class CreateTopicWithCloudStorageIngestionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateTopicWithCloudStorageIngestionSample _createTopicWithCloudStorageIngestionSample;

    public CreateTopicWithCloudStorageIngestionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createTopicWithCloudStorageIngestionSample = new CreateTopicWithCloudStorageIngestionSample();
    }

    [Fact]
    public void CreateTopicWithCloudStorageIngestion()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        string bucket = _pubsubFixture.CloudStorageBucketName;
        string inputFormat = "text";
        string textDelimiter = "\n";
        string matchGlob = "**.txt";
        DateTimeOffset minimumObjectCreateTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Topic createdTopic = _createTopicWithCloudStorageIngestionSample.CreateTopicWithCloudStorageIngestion(_pubsubFixture.ProjectId, topicId, bucket, inputFormat, textDelimiter, matchGlob, minimumObjectCreateTime);

        // Confirm that the created topic and topic retrieved by ID have the same ingestion settings.
        Topic retrievedTopic = _pubsubFixture.GetTopic(topicId);
        Assert.Equal(createdTopic.IngestionDataSourceSettings, retrievedTopic.IngestionDataSourceSettings);

        // Confirm that all Cloud Storage Ingestion params are equal to expected values
        Assert.Equal(bucket, createdTopic.IngestionDataSourceSettings.CloudStorage.Bucket);
        Assert.NotNull(createdTopic.IngestionDataSourceSettings.CloudStorage.TextFormat);
        Assert.Equal(textDelimiter, createdTopic.IngestionDataSourceSettings.CloudStorage.TextFormat.Delimiter);
        Assert.Equal(matchGlob, createdTopic.IngestionDataSourceSettings.CloudStorage.MatchGlob);
        Assert.Equal(Timestamp.FromDateTimeOffset(minimumObjectCreateTime), createdTopic.IngestionDataSourceSettings.CloudStorage.MinimumObjectCreateTime);
    }
}
