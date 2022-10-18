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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishMessageWithCustomAttributesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessageWithCustomAttributesAsyncSample _publishMessageWithCustomAttributesAsyncSample;
    private readonly PullMessagesWithCustomAttributesAsyncSample _pullMessagesWithCustomAttributesAsyncSample;
    public PublishMessageWithCustomAttributesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessageWithCustomAttributesAsyncSample = new PublishMessageWithCustomAttributesAsyncSample();
        _pullMessagesWithCustomAttributesAsyncSample = new PullMessagesWithCustomAttributesAsyncSample();
    }

    [Fact]
    public async Task PublishMessageWithCustomAttributesAsync()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForPublishMessageWithCustomAttributesAsync{randomName}";
        string subscriptionId = $"testSubscriptionForPublishMessageWithCustomAttributesAsync{randomName}";
        string message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessageWithCustomAttributesAsyncSample.PublishMessageWithCustomAttributesAsync(_pubsubFixture.ProjectId, topicId, message);

        var messages = await _pullMessagesWithCustomAttributesAsyncSample.PullMessagesWithCustomAttributesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Contains(messages, m => m.Attributes.Keys.Contains("year") && m.Attributes.Values.Contains("2020"));
    }
}
