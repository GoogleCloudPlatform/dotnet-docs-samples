﻿// Copyright 2022 Google Inc.
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

// [START pubsub_create_subscription_with_exactly_once_delivery]

using Google.Cloud.PubSub.V1;
using Grpc.Core;

public class CreateSubscriptionWithExactlyOnceDeliverySample
{
    public Subscription CreateSubscriptionWithExactlyOnceDelivery(string projectId, string topicId, string subscriptionId)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

        var subscriptionRequest = new Subscription
        {
            SubscriptionName = subscriptionName,
            TopicAsTopicName = topicName,
            EnableExactlyOnceDelivery = true
        };

        Subscription subscription = null;

        try
        {
            subscription = subscriber.CreateSubscription(subscriptionRequest);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }
        return subscription;
    }
}
// [END pubsub_create_subscription_with_exactly_once_delivery]
