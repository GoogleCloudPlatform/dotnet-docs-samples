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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishMessageWithRetrySettingsAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessageWithRetrySettingsAsyncSample _publishMessageWithRetrySettingsAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PublishMessageWithRetrySettingsAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessageWithRetrySettingsAsyncSample = new PublishMessageWithRetrySettingsAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task PublishMessageWithRetrySettingsAsync()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessageWithRetrySettingsAsyncSample
            .PublishMessageWithRetrySettingsAsync(_pubsubFixture.ProjectId, topicId, "Hello World!");

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var messageCount = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, messageCount);
        });
    }
}
