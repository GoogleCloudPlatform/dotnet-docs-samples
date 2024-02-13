// Copyright 2022 Google Inc.
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

// [START pubsub_subscriber_exactly_once]

using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.PubSub.V1.SubscriberClient;

public class ExactlyOnceDeliverySubscriberAsyncSample
{
    public async Task<IEnumerable<string>> ExactlyOnceDeliverySubscriberAsync(string projectId, string subscriptionId)
    {
        // subscriptionId should be the ID of an exactly-once delivery subscription.
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        // To get the status of ACKnowledge (ACK) or Not ACKnowledge (NACK) request in exactly once delivery subscriptions,
        // create a subscription handler that inherits from Google.Cloud.PubSub.V1.SubscriptionHandler. 
        // For more information see Google.Cloud.PubSub.V1.SubscriptionHandler reference docs here:
        // https://cloud.google.com/dotnet/docs/reference/Google.Cloud.PubSub.V1/latest/Google.Cloud.PubSub.V1.SubscriptionHandler
        var subscriptionHandler = new SampleSubscriptionHandler();
        Task subscriptionTask = subscriber.StartAsync(subscriptionHandler);
        // The subscriber will be running until it is stopped.
        await Task.Delay(5000);
        await subscriber.StopAsync(CancellationToken.None);
        // Let's make sure that the start task finished successfully after the call to stop.
        await subscriptionTask;
        return subscriptionHandler.SuccessfulAckedIds;
    }

    // Sample handler to handle messages and ACK/NACK responses.
    public class SampleSubscriptionHandler : SubscriptionHandler
    {
        public ConcurrentBag<string> SuccessfulAckedIds { get; } = new ConcurrentBag<string>();

        /// <summary>
        /// The function that processes received messages. It should be thread-safe.
        /// Return <see cref="Reply.Ack"/> to ACKnowledge the message (meaning it won't be received again).
        /// Return <see cref="Reply.Nack"/> to Not ACKnowledge the message (meaning it will be received again).
        /// From the point of view of message acknowledgement, throwing an exception is equivalent to returning <see cref="Reply.Nack"/>.
        /// </summary>
        public override async Task<Reply> HandleMessage(PubsubMessage message, CancellationToken cancellationToken)
        {
            string text = message.Data.ToStringUtf8();
            Console.WriteLine($"Message {message.MessageId}: {text}");
            return await Task.FromResult(Reply.Ack);
        }

        /// <summary>
        /// This method will receive responses for all acknowledge requests.
        /// </summary>
        public override void HandleAckResponses(IReadOnlyList<AckNackResponse> responses)
        {
            foreach (var response in responses)
            {
                if (response.Status == AcknowledgementStatus.Success)
                {
                    SuccessfulAckedIds.Add(response.MessageId);
                }

                string result = response.Status switch
                {
                    AcknowledgementStatus.Success => $"MessageId {response.MessageId} successfully acknowledged.",
                    AcknowledgementStatus.PermissionDenied => $"MessageId {response.MessageId} failed to acknowledge due to a permission denied error.",
                    AcknowledgementStatus.FailedPrecondition => $"MessageId {response.MessageId} failed to acknowledge due to a failed precondition.",
                    AcknowledgementStatus.InvalidAckId => $"MessageId {response.MessageId} failed to acknowledge due an invalid or expired AckId.",
                    AcknowledgementStatus.Other => $"MessageId {response.MessageId} failed to acknowledge due to an unknown reason.",
                    _ => $"Unknown acknowledgement status for messageId {response.MessageId}."
                };

                Console.WriteLine(result);
            }
        }
    }
}
// [END pubsub_subscriber_exactly_once]
