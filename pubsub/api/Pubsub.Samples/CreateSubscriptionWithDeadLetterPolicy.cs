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

// [START pubsub_dead_letter_create_subscription]

using Google.Cloud.PubSub.V1;
using System;

public class CreateSubscriptionWithDeadLetterPolicySample
{
    public Subscription CreateSubscriptionWithDeadLetterPolicy(string projectId, string topicId, string subscriptionId, string deadLetterTopicId)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
        // This is the subscription you want to create with a dead letter policy.
        var subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        // This is an existing topic that you want to attach the subscription with dead letter policy to.
        var topicName = TopicName.FromProjectTopic(projectId, topicId);
        // This is an existing topic that the subscription with dead letter policy forwards dead letter messages to.
        var deadLetterTopic = TopicName.FromProjectTopic(projectId, deadLetterTopicId).ToString();
        var subscriptionRequest = new Subscription
        {
            SubscriptionName = subscriptionName,
            TopicAsTopicName = topicName,
            DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadLetterTopic,
                // The maximum number of times that the service attempts to deliver a
                // message before forwarding it to the dead letter topic. Must be [5-100].
                MaxDeliveryAttempts = 10
            },
            AckDeadlineSeconds = 30
        };

        var subscription = subscriber.CreateSubscription(subscriptionRequest);
        Console.WriteLine("Created subscription: " + subscription.SubscriptionName.SubscriptionId);
        Console.WriteLine($"It will forward dead letter messages to: {subscription.DeadLetterPolicy.DeadLetterTopic}");
        Console.WriteLine($"After {subscription.DeadLetterPolicy.MaxDeliveryAttempts} delivery attempts.");
        // Remember to attach a subscription to the dead letter topic because
        // messages published to a topic with no subscriptions are lost.
        return subscription;
    }
}
// [END pubsub_dead_letter_create_subscription]
