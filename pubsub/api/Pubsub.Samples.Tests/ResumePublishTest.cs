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
using Xunit;

[Collection(nameof(PubsubFixture))]
public class ResumePublishTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly ResumePublishSample _ResumePublishSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public ResumePublishTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _ResumePublishSample = new ResumePublishSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async void PublishMessage()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForMessageCreation{randomName}";
        string subscriptionId = $"testSubscriptionForMessageCreation{randomName}";

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        Dictionary<string, string> messages = new Dictionary<string, string> { {"Hello World!", "Key1"}, {"Good day.", "Key2"}, {"Bye bye.", "Key1"} };

        var output = await _ResumePublishSample.PublishOrderedMessagesAsync(_pubsubFixture.ProjectId, topicId, messages);
        Assert.Equal(messages.Count, output);

        // Pull the Message to confirm it is valid
        var pulledMessages = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
        Assert.True(result > 0);
    }
}
