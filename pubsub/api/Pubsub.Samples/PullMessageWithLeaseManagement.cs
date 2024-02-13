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

// [START pubsub_subscriber_sync_pull_with_lease]

using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;

public class PullMessageWithLeaseManagementSample
{
    public int PullMessageWithLeaseManagement(string projectId, string subscriptionId, bool acknowledge)
    {
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();

        var ackIds = new List<string>();
        try
        {
            PullResponse response = subscriberClient.Pull(subscriptionName, maxMessages: 20);

            // Print out each received message.
            foreach (ReceivedMessage msg in response.ReceivedMessages)
            {
                ackIds.Add(msg.AckId);
                string text = msg.Message.Data.ToStringUtf8();
                Console.WriteLine($"Message {msg.Message.MessageId}: {text}");

                // Modify the ack deadline of each received message from the default 10 seconds to 30.
                // This prevents the server from redelivering the message after the default 10 seconds
                // have passed.
                subscriberClient.ModifyAckDeadline(subscriptionName, new List<string> { msg.AckId }, 30);
            }
            // If acknowledgement required, send to server.
            if (acknowledge && ackIds.Count > 0)
            {
                subscriberClient.Acknowledge(subscriptionName, ackIds);
            }
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
        {
            // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.
        }
        return ackIds.Count;
    }
}
// [END pubsub_subscriber_sync_pull_with_lease]
