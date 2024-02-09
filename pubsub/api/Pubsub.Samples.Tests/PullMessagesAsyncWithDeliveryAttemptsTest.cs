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

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PullMessagesAsyncWithDeliveryAttemptsTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesAsyncWithDeliveryAttemptsSample _pullMessagesAsyncWithDeliveryAttemptsSample;
    private readonly CreateSubscriptionWithDeadLetterPolicySample _createSubscriptionWithDeadLetterPolicySample;

    public PullMessagesAsyncWithDeliveryAttemptsTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesAsyncWithDeliveryAttemptsSample = new PullMessagesAsyncWithDeliveryAttemptsSample();
        _createSubscriptionWithDeadLetterPolicySample = new CreateSubscriptionWithDeadLetterPolicySample();
    }

    [Fact]
    public async Task PullMessagesAsyncWithDeliveryAttempts()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        var message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _createSubscriptionWithDeadLetterPolicySample.CreateSubscriptionWithDeadLetterPolicy(
           _pubsubFixture.ProjectId, topicId, subscriptionId, _pubsubFixture.DeadLetterTopic);

        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new List<string> { message });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var deliveryAttempt = await _pullMessagesAsyncWithDeliveryAttemptsSample.PullMessagesAsyncWithDeliveryAttempts(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.True(deliveryAttempt > 0);
        });
    }
}
