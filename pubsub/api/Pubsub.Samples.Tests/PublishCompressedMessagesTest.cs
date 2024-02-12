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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishCompressedMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishCompressedMessagesAsyncSample _publishCompressedMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PublishCompressedMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishCompressedMessagesAsyncSample = new PublishCompressedMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task PublishCompressedMessagesAsync()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        string messageText = "Hello world! This message will be compressed.";

        await _publishCompressedMessagesAsyncSample.PublishCompressedMessagesAsync(_pubsubFixture.ProjectId, topicId, messageText);

        // Pull the message to confirm it's been published correctly.
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, result);
        });
    }
}
