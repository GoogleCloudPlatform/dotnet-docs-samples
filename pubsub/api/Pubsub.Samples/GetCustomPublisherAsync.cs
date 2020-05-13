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

using Google.Cloud.PubSub.V1;
using System;
using System.Threading.Tasks;

public class GetCustomPublisherAsyncSample
{
    /// <summary>
    /// Create a PublisherClient with custom batch thresholds.
    /// </summary>
    public async Task<PublisherClient> GetCustomPublisherAsync(string projectId, string topicId)
    {
        PublisherClient publisher = await PublisherClient.CreateAsync(
            new TopicName(projectId, topicId),
            settings: new PublisherClient.Settings
            {
                BatchingSettings = new Google.Api.Gax.BatchingSettings(
                    elementCountThreshold: 100,
                    byteCountThreshold: 10240,
                    delayThreshold: TimeSpan.FromSeconds(3))
            });
        return publisher;
    }
}
// [END pubsub_publisher_batch_settings]
