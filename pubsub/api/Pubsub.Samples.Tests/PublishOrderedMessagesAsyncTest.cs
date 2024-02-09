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
public class PublishOrderedMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishOrderedMessagesAsyncSample _publishOrderedMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PublishOrderedMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishOrderedMessagesAsyncSample = new PublishOrderedMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task PublishOrderedMessagesAsync()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        List<(string, string)> messages = new List<(string, string)> { ("Key1", "Hello World!"), ("Key2", "Good day."), ("Key1", "Bye bye") };

        var publishedMessages = await _publishOrderedMessagesAsyncSample.PublishOrderedMessagesAsync(_pubsubFixture.ProjectId, topicId, messages);
        Assert.Equal(messages.Count, publishedMessages);

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var messagesPulled = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
            Assert.True(messagesPulled > 0);
        });
    }
}
