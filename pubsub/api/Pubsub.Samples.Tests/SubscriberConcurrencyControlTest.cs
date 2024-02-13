// Copyright 2024 Google Inc.
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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class SubscriberConcurrencyControlTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly SubscriberConcurrencyControlSample _subscriberConcurrencyControlSample;

    public SubscriberConcurrencyControlTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _subscriberConcurrencyControlSample = new SubscriberConcurrencyControlSample();
    }

    [Fact]
    public async Task SubscribeWithConcurrencyControl()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        var messages = Enumerable.Range(0, 100).Select(index => $"Message {index}").ToList();
        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, messages);

        // Pull and acknowledge the messages
        var result = await _subscriberConcurrencyControlSample.SubscribeWithConcurrencyControlAsync(_pubsubFixture.ProjectId, subscriptionId);

        Assert.Equal(messages.Count, result);
    }
}
