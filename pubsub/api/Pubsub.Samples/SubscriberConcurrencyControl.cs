// Copyright 2024 Google LLC
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

// [START pubsub_subscriber_concurrency_control]

using Google.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SubscriberConcurrencyControlSample
{
    public async Task<int> SubscribeWithConcurrencyControlAsync(string projectId, string subscriptionId)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

        SubscriberClient subscriber = await new SubscriberClientBuilder
        {
            SubscriptionName = subscriptionName,
            // Normally the number of clients depends on the number of processors.
            // Here we explicitly request 2 concurrent clients instead.
            ClientCount = 2
        }.BuildAsync();

        int count = 0;
        Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancellationToken) =>
        {
            string text = message.Data.ToStringUtf8();
            Console.WriteLine($"Received message: {text}");
            Interlocked.Increment(ref count);
            return Task.FromResult(SubscriberClient.Reply.Ack);
        });
        // Run for 10 seconds.
        await Task.Delay(10_000);
        await subscriber.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return count;
    }
}

// [END pubsub_subscriber_concurrency_control]
