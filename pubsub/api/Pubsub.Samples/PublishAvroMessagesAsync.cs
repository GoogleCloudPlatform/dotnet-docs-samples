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

// [START pubsub_publish_avro_records]

using Avro.IO;
using Avro.Specific;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PublishAvroMessagesAsyncSample
{
    public async Task<int> PublishAvroMessagesAsync(string projectId, string topicId, IEnumerable<AvroUtilities.State> messageStates)
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
                        using (var ms = new MemoryStream())
                        {
                            var encoder = new BinaryEncoder(ms);
                            var writer = new SpecificDefaultWriter(state.Schema);
                            writer.Write(state, encoder);
                            messageId = await publisher.PublishAsync(ms.ToArray());
                        }
                        break;
                    case Encoding.Json:
                        var jsonMessage = AvroUtilities.StateUtils.StateToJsonString(state);
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
// [END pubsub_publish_avro_records]
