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

// [START pubsub_create_topic_with_cloud_storage_ingestion]

using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;

public class CreateTopicWithCloudStorageIngestionSample
{
    public Topic CreateTopicWithCloudStorageIngestion(string projectId, string topicId, string bucket, string inputFormat, string textDelimiter, string matchGlob, string minimumObjectCreateTime)
    {

        IngestionDataSourceSettings.Types.CloudStorage cloudStorageSettings = new IngestionDataSourceSettings.Types.CloudStorage { Bucket = bucket };

        switch (inputFormat)
        {
            case "text":
                cloudStorageSettings.TextFormat = new IngestionDataSourceSettings.Types.CloudStorage.Types.TextFormat
                {
                    Delimiter = textDelimiter
                };
                break;
            case "avro":
                cloudStorageSettings.AvroFormat = new IngestionDataSourceSettings.Types.CloudStorage.Types.AvroFormat();
                break;
            case "pubsub_avro":
                cloudStorageSettings.PubsubAvroFormat = new IngestionDataSourceSettings.Types.CloudStorage.Types.PubSubAvroFormat();
                break;
            default:
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"inputFormat must be in ('text', 'avro', 'pubsub_avro'); got value: {inputFormat}"));
        }

        if (!string.IsNullOrEmpty(matchGlob))
        {
            cloudStorageSettings.MatchGlob = matchGlob;
        }

        if (!string.IsNullOrEmpty(minimumObjectCreateTime))
        {
            cloudStorageSettings.MinimumObjectCreateTime = Timestamp.FromDateTime(DateTime.Parse(minimumObjectCreateTime));
        }

        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        Topic topic = new Topic()
        {
            Name = TopicName.FormatProjectTopic(projectId, topicId),
            IngestionDataSourceSettings = new IngestionDataSourceSettings() { CloudStorage = cloudStorageSettings }
        };
        Topic createdTopic = publisher.CreateTopic(topic);
        Console.WriteLine($"Topic {createdTopic.Name} created.");

        return createdTopic;
    }
}

// [END pubsub_create_topic_with_cloud_storage_ingestion]
