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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class RemoveDeadLetterPolicyTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateSubscriptionWithDeadLetterPolicySample _createSubscriptionWithDeadLetterPolicySample;
    private readonly RemoveDeadLetterPolicySample _removeDeadLetterPolicySample;

    public RemoveDeadLetterPolicyTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createSubscriptionWithDeadLetterPolicySample = new CreateSubscriptionWithDeadLetterPolicySample();
        _removeDeadLetterPolicySample = new RemoveDeadLetterPolicySample();
    }

    [Fact]
    public void RemoveDeadLetterPolicy()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _createSubscriptionWithDeadLetterPolicySample.CreateSubscriptionWithDeadLetterPolicy(_pubsubFixture.ProjectId, topicId, subscriptionId, _pubsubFixture.DeadLetterTopic);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        _removeDeadLetterPolicySample.RemoveDeadLetterPolicy(_pubsubFixture.ProjectId, topicId, subscriptionId);
        var subscription = _pubsubFixture.GetSubscription(subscriptionId);

        Assert.Null(subscription.DeadLetterPolicy);
    }
}
