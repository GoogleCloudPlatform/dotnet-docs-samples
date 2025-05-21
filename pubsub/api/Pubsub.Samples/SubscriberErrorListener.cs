// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START pubsub_subscriber_error_listener]

using Google.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SubscriberErrorListenerSample
{
    public async Task<int> SubscriberErrorListener(string projectId, string subscriptionId)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);

        int messageCount = 0;
        try
        {
            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = message.Data.ToStringUtf8();
                Console.WriteLine($"Message {message.MessageId}: {text}");
                Interlocked.Increment(ref messageCount);
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Wait for the StartAsync call to finish
            await startTask;
        }
        // Catch unrecoverable failures.
        catch (Exception e)
        {
            Console.WriteLine($"Message: {e.Message}");
            Console.WriteLine($"StackTrace: {e.StackTrace}");
        }
        return messageCount;
    }
}

// [END pubsub_subscriber_error_listener]