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
using System;
using System.Linq;
using System.Text;

public class PullMessagesSyncSample
{
    public static object PullMessagesSync(string projectId,
    string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = new SubscriptionName(projectId,
            subscriptionId);
        SubscriberServiceApiClient subscriberClient =
            SubscriberServiceApiClient.Create();
        // Pull messages from server,
        // allowing an immediate response if there are no messages.
        PullResponse response = subscriberClient.Pull(
            subscriptionName, returnImmediately: true, maxMessages: 20);
        // Print out each received message.
        foreach (ReceivedMessage msg in response.ReceivedMessages)
        {
            string text = Encoding.UTF8.GetString(msg.Message.Data.ToArray());
            Console.WriteLine($"Message {msg.Message.MessageId}: {text}");
        }
        // If acknowledgement required, send to server.
        if (acknowledge)
        {
            subscriberClient.Acknowledge(subscriptionName,
                response.ReceivedMessages.Select(msg => msg.AckId));
        }
        return 0;
    }
}
// [END pubsub_subscriber_sync_pull]
