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

// [START pubsub_subscriber_sync_pull]

using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading;

public class PullMessagesSyncSample
{
    public int PullMessagesSync(string projectId, string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
        int messageCount = 0;
        try
        {
            // Pull messages from server,
            // allowing an immediate response if there are no messages.
            PullResponse response = subscriberClient.Pull(subscriptionName, maxMessages: 20);
            // Print out each received message.
            foreach (ReceivedMessage msg in response.ReceivedMessages)
            {
                string text = msg.Message.Data.ToStringUtf8();
                Console.WriteLine($"Message {msg.Message.MessageId}: {text}");
                Interlocked.Increment(ref messageCount);
            }
            // If acknowledgement required, send to server.
            if (acknowledge && messageCount > 0)
            {
                subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId));
            }
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
        {
            // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
        }
        return messageCount;
    }
}
// [END pubsub_subscriber_sync_pull]
