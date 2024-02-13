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

// [START pubsub_subscriber_async_pull_custom_attributes]

using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PullMessagesWithCustomAttributesAsyncSample
{
    public async Task<List<PubsubMessage>> PullMessagesWithCustomAttributesAsync(string projectId, string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        var messages = new List<PubsubMessage>();
        Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            messages.Add(message);
            string text = message.Data.ToStringUtf8();
            Console.WriteLine($"Message {message.MessageId}: {text}");
            if (message.Attributes != null)
            {
                foreach (var attribute in message.Attributes)
                {
                    Console.WriteLine($"{attribute.Key} = {attribute.Value}");
                }
            }
            return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
        });
        // Run for 7 seconds.
        await Task.Delay(7000);
        await subscriber.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return messages;
    }
}
// [END pubsub_subscriber_async_pull_custom_attributes]
