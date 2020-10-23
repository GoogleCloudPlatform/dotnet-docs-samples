// Copyright 2020 Google Inc.
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

// [START pubsub_publish_custom_attributes]

using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System;
using System.Threading.Tasks;

public class PublishMessageWithCustomAttributesAsyncSample
{
    public async Task PublishMessageWithCustomAttributesAsync(string projectId, string topicId, string messageText)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

        var pubsubMessage = new PubsubMessage
        {
            // The data is any arbitrary ByteString. Here, we're using text.
            Data = ByteString.CopyFromUtf8(messageText),
            // The attributes provide metadata in a string-to-string dictionary.
            Attributes =
            {
                { "year", "2020" },
                { "author", "unknown" }
            }
        };
        string message = await publisher.PublishAsync(pubsubMessage);
        Console.WriteLine($"Published message {message}");
    }
}
// [END pubsub_publish_custom_attributes]
