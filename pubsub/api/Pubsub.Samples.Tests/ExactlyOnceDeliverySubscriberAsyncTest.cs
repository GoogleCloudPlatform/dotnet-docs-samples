﻿// Copyright 2022 Google Inc.
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
public class ExactlyOnceDeliverySubscriberAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly ExactlyOnceDeliverySubscriberAsyncSample _exactlyOnceDeliverySubscriberAsyncSample;

    public ExactlyOnceDeliverySubscriberAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _exactlyOnceDeliverySubscriberAsyncSample = new ExactlyOnceDeliverySubscriberAsyncSample();
    }

    [Fact]
    public async Task ExactlyOnceDeliverySubscriberAsync()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForExactlyOnceDelivery{randomName}";
        string subscriptionId = $"testSubscriptionForExactlyOnceDelivery{randomName}";
        var message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateExactlyOnceDeliverySubscription(topicId, subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { message });

        // Validate that the published message is received exactly once.
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var successfulIds = await _exactlyOnceDeliverySubscriberAsyncSample.ExactlyOnceDeliverySubscriberAsync(_pubsubFixture.ProjectId, subscriptionId);
            Assert.Single(successfulIds);
        });
    }
}
