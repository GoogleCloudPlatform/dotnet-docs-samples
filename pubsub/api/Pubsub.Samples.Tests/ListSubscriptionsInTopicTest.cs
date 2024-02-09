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

using Google.Cloud.PubSub.V1;
using System.Linq;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class ListSubscriptionsInTopicTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly ListSubscriptionsInTopicSample _listSubscriptionsInTopicSample;

    public ListSubscriptionsInTopicTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _listSubscriptionsInTopicSample = new ListSubscriptionsInTopicSample();
    }

    [Fact]
    public void ListSubscriptionsInTopic()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        var subscriptions = _listSubscriptionsInTopicSample.ListSubscriptionsInTopic(_pubsubFixture.ProjectId, topicId).ToList();

        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_pubsubFixture.ProjectId, subscriptionId);
        Assert.Contains(subscriptions, s => s == subscriptionName.ToString());
    }
}
