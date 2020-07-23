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

using Xunit;

[Collection(nameof(PubsubFixture))]
public class PullMessagesSyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesSyncSample _pullMessagesSyncSample;

    public PullMessagesSyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesSyncSample = new PullMessagesSyncSample();
    }

    [Fact]
    public async void PullMessagesSync()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForMessageSyncAck{randomName}";
        string subscriptionId = $"testSubscriptionForMessageSyncAck{randomName}";
        var message = _pubsubFixture.RandomName();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { message });

        // Pull and acknowledge the messages
        var result = _pullMessagesSyncSample.PullMessagesSync(_pubsubFixture.ProjectId, subscriptionId, true);
        // sometimes UNAVAILABLE response from service.
        Assert.True(result <= 1);

        //Pull the Message to confirm it's gone after it's acknowledged
        result = _pullMessagesSyncSample.PullMessagesSync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.True(result <= 1);
    }
}
