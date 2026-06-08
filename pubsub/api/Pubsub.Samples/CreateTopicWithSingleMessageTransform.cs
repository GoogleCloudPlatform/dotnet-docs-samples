// Copyright 2026 Google LLC.
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

// [START pubsub_create_topic_with_smt]

using Google.Cloud.PubSub.V1;
using System;

public class CreateTopicWithSingleMessageTransformSample
{
    public Topic CreateTopicWithSingleMessageTransform(string projectId, string topicId)
    {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        var topicName = TopicName.FromProjectTopic(projectId, topicId);

        MessageTransform removeSSNTransform = new MessageTransform
        {
            JavascriptUdf = new JavaScriptUDF
            {
                FunctionName = "redactSsn",
                Code = "function redactSsn(message, metadata) {"
                + "   const data = JSON.parse(message.data);"
                + "   delete data['ssn'];"
                + "   message.data = JSON.stringify(data);"
                + "   return message;"
                + "}"
            },
        };

        Topic topic = publisher.CreateTopic(new Topic()
        {
            TopicName = topicName,
            MessageTransforms = { removeSSNTransform }
        });
        Console.WriteLine($"Topic {topic.Name} created with SMT.");

        return topic;
    }
}
// [END pubsub_create_topic_with_smt]
