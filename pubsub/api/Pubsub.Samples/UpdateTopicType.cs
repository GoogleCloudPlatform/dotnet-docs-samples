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

// [START pubsub_update_topic_type]

using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateTopicTypeSample
{
    public Topic UpdateTopicType(string projectId, string topicId, string streamArn, string consumerArn, string awsRoleArn, string gcpServiceAccount)
    {
        // Define settings for Kinesis ingestion
        IngestionDataSourceSettings ingestionDataSourceSettings = new IngestionDataSourceSettings
        {
            AwsKinesis = new IngestionDataSourceSettings.Types.AwsKinesis
            {
                AwsRoleArn = awsRoleArn,
                ConsumerArn = consumerArn,
                GcpServiceAccount = gcpServiceAccount,
                StreamArn = streamArn
            }
        };
        // Construct Topic with Kinesis ingestion settings
        Topic topic = new Topic()
        {
            Name = TopicName.FormatProjectTopic(projectId, topicId),
            IngestionDataSourceSettings = ingestionDataSourceSettings
        };

        PublisherServiceApiClient client = PublisherServiceApiClient.Create();
        UpdateTopicRequest updateTopicRequest = new UpdateTopicRequest
        {
            Topic = topic,
            //Construct a field mask to indicate which field to update in the topic.
            UpdateMask = new FieldMask { Paths = { "ingestion_data_source_settings" } }
        };
        Topic updatedTopic = client.UpdateTopic(updateTopicRequest);
        Console.WriteLine($"Topic {topic.Name} updated.");

        return updatedTopic;
    }
}

// [END pubsub_update_topic_type]
