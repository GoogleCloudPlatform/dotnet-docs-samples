// Copyright 2021 Google Inc.
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

// [START pubsub_publish_proto_messages]

using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PublishProtoMessagesAsyncSample
{
    public async Task<int> PublishProtoMessagesAsync(string projectId, string topicId, IEnumerable<Utilities.State> messageStates)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

        PublisherServiceApiClient publishApi = PublisherServiceApiClient.Create();
        var topic = publishApi.GetTopic(topicName);

        int publishedMessageCount = 0;
        var publishTasks = messageStates.Select(async state =>
        {
            try
            {
                string messageId = null;
                switch (topic.SchemaSettings.Encoding)
                {
                    case Encoding.Binary:
                        var binaryMessage = state.ToByteString();
                        messageId = await publisher.PublishAsync(binaryMessage);
                        break;
                    case Encoding.Json:
                        var jsonMessage = JsonFormatter.Default.Format(state);
                        messageId = await publisher.PublishAsync(jsonMessage);
                        break;
                }
                Console.WriteLine($"Published message {messageId}");
                Interlocked.Increment(ref publishedMessageCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred when publishing message {state}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
}
// [END pubsub_publish_proto_messages]
