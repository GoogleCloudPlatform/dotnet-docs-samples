// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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
public class OptimisticSubscribeTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly OptimisticSubscribeSample _optimisticSubscribeSample;

    public OptimisticSubscribeTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _optimisticSubscribeSample = new OptimisticSubscribeSample();
    }

    [Fact]
    public async Task SubscribeOptimistically()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        var messages = new List<string>() { "Message 0" };

        _pubsubFixture.CreateTopic(topicId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, messages);

        var result = await _optimisticSubscribeSample.OptimisticSubscribe(_pubsubFixture.ProjectId, topicId, subscriptionId);

        Assert.Equal(messages.Count, result);
    }
}
