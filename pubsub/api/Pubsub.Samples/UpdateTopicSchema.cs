// Copyright 2024 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START pubsub_update_topic_schema]

using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateTopicSchemaSample
{
    public Topic UpdateTopicSchema(string projectId, string topicId, string firstRevisionId, string lastRevisionId)
    {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        var topicName = TopicName.FromProjectTopic(projectId, topicId);
        Topic topic = new Topic
        {
            TopicName = topicName,
            SchemaSettings = new SchemaSettings
            {
                FirstRevisionId = firstRevisionId,
                LastRevisionId = lastRevisionId
            }
        };
        FieldMask updateMask = new FieldMask { Paths = { "schema_settings.first_revision_id", "schema_settings.last_revision_id" } };

        Topic receivedTopic = publisher.UpdateTopic(topic, updateMask);
        Console.WriteLine($"Topic {topic.Name} updated.");
        return receivedTopic;
    }
}
// [END pubsub_update_topic_schema]
