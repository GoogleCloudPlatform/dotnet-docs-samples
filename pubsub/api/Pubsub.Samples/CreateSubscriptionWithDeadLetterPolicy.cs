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
using Grpc.Core;

public class CreateSubscriptionWithDeadLetterPolicySample
{
    public Subscription CreateSubscriptionWithDeadLetterPolicy(string projectId, string subscriptionId, string topicId, string deadLetterTopicId)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();

        Subscription subscription = null;
        try
        {
            var subscriptionRequest = new Subscription()
            {
                SubscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId),
                TopicAsTopicName = TopicName.FromProjectTopic(projectId, topicId),
                DeadLetterPolicy = new DeadLetterPolicy
                {
                    DeadLetterTopic = TopicName.FromProjectTopic(projectId, deadLetterTopicId).ToString(),
                    MaxDeliveryAttempts = 10
                },
                AckDeadlineSeconds = 30
            };

            subscription = subscriber.CreateSubscription(subscriptionRequest);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }
        return subscription;
    }
}
// [END pubsub_dead_letter_create_subscription]
