// Copyright 2023 Google Inc.
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

namespace Pubsub.Samples.Tests;

[Collection(nameof(PubsubFixture))]
public class PullMessagesAsyncWithErrorListenTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesAsyncWithErrorListen _pullMessagesWithErrorListenSample;

    public PullMessagesAsyncWithErrorListenTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesWithErrorListenSample = new PullMessagesAsyncWithErrorListen();
    }

    [Fact]
    public async Task PullMessagesWithErrorAsyncTest()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForMessageAck{randomName}";
        string subscriptionId = $"testSubscriptionForMessageAck{randomName}";
        var message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        // Intentionally not creating the subscription, want pull to throw some unrecoverable exception.
        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { message });
        var result = await _pullMessagesWithErrorListenSample.PullMessagesAsyncListenError(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }
}
