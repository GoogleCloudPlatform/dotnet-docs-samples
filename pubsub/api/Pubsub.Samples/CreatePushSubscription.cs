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

// [START pubsub_create_push_subscription]

using Google.Cloud.PubSub.V1;

public class CreatePushSubscriptionSample
{
    public Subscription CreatePushSubscription(string projectId, string topicId, string subscriptionId, string pushEndpoint)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

        PushConfig pushConfig = new PushConfig { PushEndpoint = pushEndpoint };

        // The approximate amount of time in seconds (on a best-effort basis) Pub/Sub waits for the
        // subscriber to acknowledge receipt before resending the message.
        var ackDeadlineSeconds = 60;
        var subscription = subscriber.CreateSubscription(subscriptionName, topicName, pushConfig, ackDeadlineSeconds);
        return subscription;
    }
}
// [END pubsub_create_push_subscription]
