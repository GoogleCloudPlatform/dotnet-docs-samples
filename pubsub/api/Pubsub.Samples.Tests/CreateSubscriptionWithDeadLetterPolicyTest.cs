﻿// Copyright 2020 Google Inc.
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
public class CreateSubscriptionWithDeadLetterPolicyTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateSubscriptionWithDeadLetterPolicySample _createSubscriptionWithDeadLetterPolicySample;

    public CreateSubscriptionWithDeadLetterPolicyTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createSubscriptionWithDeadLetterPolicySample = new CreateSubscriptionWithDeadLetterPolicySample();
    }

    [Fact]
    public void CreateSubscriptionWithDeadLetterPolicy()
    {
        string topicId = "testTopicForDeadLetterPolicySubscriptionCreation" + _pubsubFixture.RandomName();
        string subscriptionId = "testSubscriptionForDeadLetterPolicySubscriptionCreation" + _pubsubFixture.RandomName();
        string deadLetterTopicId = "testTopicForDeadLetter" + _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateTopic(deadLetterTopicId);
        var newlyCreatedSubscription = _createSubscriptionWithDeadLetterPolicySample.CreateSubscriptionWithDeadLetterPolicy(_pubsubFixture.ProjectId, subscriptionId, topicId, deadLetterTopicId);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);
        var subscription = _pubsubFixture.GetSubscription(subscriptionId);

        Assert.Equal(newlyCreatedSubscription, subscription);
    }
}
