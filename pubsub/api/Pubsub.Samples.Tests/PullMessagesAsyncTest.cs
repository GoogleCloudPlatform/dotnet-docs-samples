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
public class PullMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PullMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task PullMessagesAsync()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        var message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { message });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, result);
        });

        //Pull the Message to confirm it's gone after it's acknowledged
        var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }
}
