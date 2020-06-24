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

// [START pubsub_publisher_batch_settings]

using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PublishBatchedMessagesAsyncSample
{
    public async Task<int> PublishBatchMessagesAsync(string projectId, string topicId, IEnumerable<string> messageTexts)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);

        // Default Settings:
        // byteCountThreshold: 1000000
        // elementCountThreshold: 100
        // delayThreshold: 10 milliseconds
        var customSettings = new PublisherClient.Settings
        {
            BatchingSettings = new BatchingSettings(
                elementCountThreshold: 50,
                byteCountThreshold: 10240,
                delayThreshold: TimeSpan.FromMilliseconds(500))
        };

        PublisherClient publisher = await PublisherClient.CreateAsync(topicName, settings: customSettings);

        int publishedMessageCount = 0;
        var publishTasks = messageTexts.Select(async text =>
        {
            try
            {
                string message = await publisher.PublishAsync(text);
                Console.WriteLine($"Published message {message}");
                Interlocked.Increment(ref publishedMessageCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred when publishing message {text}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
}
// [END pubsub_publisher_batch_settings]
