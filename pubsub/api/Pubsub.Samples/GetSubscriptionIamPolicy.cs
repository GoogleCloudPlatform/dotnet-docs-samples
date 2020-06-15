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

// [START pubsub_get_subscription_policy]

using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;

public class GetSubscriptionIamPolicySample
{
    public Policy GetSubscriptionIamPolicy(string projectId, string subscriptionId)
    {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        Policy policy = publisher.GetIamPolicy(subscriptionName);
        return policy;
    }
}
// [END pubsub_get_subscription_policy]
