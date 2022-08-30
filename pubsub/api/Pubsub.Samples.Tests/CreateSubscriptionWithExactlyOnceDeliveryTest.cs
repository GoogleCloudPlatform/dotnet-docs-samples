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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class CreateSubscriptionWithExactlyOnceDeliveryTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateSubscriptionWithExactlyOnceDeliverySample _createSubscriptionWithExactlyOnceDeliverySample;

    public CreateSubscriptionWithExactlyOnceDeliveryTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createSubscriptionWithExactlyOnceDeliverySample = new CreateSubscriptionWithExactlyOnceDeliverySample();
    }

    [Fact]
    public void CreateSubscriptionWithExactlyOnceDelivery()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForExactlyOnceDeliverySubscriptionCreation{randomName}";
        string subscriptionId = $"testSubscriptionForExactlyOnceDelivery{randomName}";

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);
        _createSubscriptionWithExactlyOnceDeliverySample.CreateSubscriptionWithExactlyOnceDelivery(_pubsubFixture.ProjectId, topicId, subscriptionId);
        var subscription = _pubsubFixture.GetSubscription(subscriptionId);
        Assert.True(subscription.EnableExactlyOnceDelivery);
    }
}
