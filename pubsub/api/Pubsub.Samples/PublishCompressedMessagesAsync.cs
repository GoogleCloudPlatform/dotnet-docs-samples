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

// [START pubsub_publisher_with_compression]

using Google.Cloud.PubSub.V1;
using System.Threading.Tasks;

public class PublishCompressedMessagesAsyncSample
{
    public async Task PublishCompressedMessagesAsync(string projectId, string topicId, string messageText)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);

        var customSettings = new PublisherClient.Settings
        {
            EnableCompression = true,
            // Compress any batch of messages over 10 bytes. By default,
            // the threshold is taken from PublisherClient.Settings.DefaultCompressionBytesThreshold.
            CompressionBytesThreshold = 10
        };

        PublisherClient publisher = await new PublisherClientBuilder
        {
            TopicName = topicName,
            Settings = customSettings
        }.BuildAsync();

        await publisher.PublishAsync(messageText);
    }
}
// [END pubsub_publisher_with_compression]
