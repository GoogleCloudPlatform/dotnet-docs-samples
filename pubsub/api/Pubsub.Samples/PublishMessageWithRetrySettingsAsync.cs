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

// [START pubsub_publisher_retry_settings]

using Google.Api.Gax.Grpc;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Threading.Tasks;

public class PublishMessageWithRetrySettingsAsyncSample
{
    public async Task PublishMessageWithRetrySettingsAsync(string projectId, string topicId, string messageText)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        // Retry settings control how the publisher handles retry-able failures
        var maxAttempts = 3;
        var initialBackoff = TimeSpan.FromMilliseconds(110); // default: 100 ms
        var maxBackoff = TimeSpan.FromSeconds(70); // default : 60 seconds
        var backoffMultiplier = 1.3; // default: 1.0
        var totalTimeout = TimeSpan.FromSeconds(100); // default: 600 seconds

        var publisher = await PublisherClient.CreateAsync(topicName,
               clientCreationSettings: new PublisherClient.ClientCreationSettings(
                   publisherServiceApiSettings: new PublisherServiceApiSettings
                   {
                       PublishSettings = CallSettings.FromRetry(RetrySettings.FromExponentialBackoff(
                               maxAttempts: maxAttempts,
                               initialBackoff: initialBackoff,
                               maxBackoff: maxBackoff,
                               backoffMultiplier: backoffMultiplier,
                               retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Unavailable)))
                       .WithTimeout(totalTimeout)
                   }
               )).ConfigureAwait(false);
        string message = await publisher.PublishAsync(messageText);
        Console.WriteLine($"Published message {message}");
    }
}
// [END pubsub_publisher_retry_settings]
