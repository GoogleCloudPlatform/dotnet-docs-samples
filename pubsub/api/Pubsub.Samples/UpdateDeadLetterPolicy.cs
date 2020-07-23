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

// [START pubsub_dead_letter_update_subscription]

using Google.Cloud.PubSub.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateDeadLetterPolicySample
{
    public Subscription UpdateDeadLetterPolicy(string projectId, string topicId, string subscriptionId, string deadLetterTopicId)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
        // This is an existing topic that the subscription with dead letter policy is attached to.
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
        // This is an existing subscription with a dead letter policy.
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        // This is an existing dead letter topic that the subscription with dead letter policy forwards
        // dead letter messages to.
        var deadLetterTopic = TopicName.FromProjectTopic(projectId, deadLetterTopicId).ToString();


        // Construct the subscription with the dead letter policy you expect to have after the update.
        // Here, values in the required fields (name, topic) help identify the subscription.
        var subscription = new Subscription
        {
            SubscriptionName = subscriptionName,
            TopicAsTopicName = topicName,
            DeadLetterPolicy = new DeadLetterPolicy
            {
                DeadLetterTopic = deadLetterTopic,
                MaxDeliveryAttempts = 20,
            }
        };

        var request = new UpdateSubscriptionRequest
        {
            Subscription = subscription,
            // Construct a field mask to indicate which field to update in the subscription.
            UpdateMask = new FieldMask { Paths = { "dead_letter_policy" } }
        };
        var updatedSubscription = subscriber.UpdateSubscription(request);
        return updatedSubscription;
    }
}
// [END pubsub_dead_letter_update_subscription]
