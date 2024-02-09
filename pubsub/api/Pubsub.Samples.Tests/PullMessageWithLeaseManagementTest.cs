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
public class PullMessageWithLeaseManagementTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessageWithLeaseManagementSample _pullMessageWithLeaseManagementSample;

    public PullMessageWithLeaseManagementTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessageWithLeaseManagementSample = new PullMessageWithLeaseManagementSample();
    }

    [Fact]
    public async Task PullMessageWithLeaseManagement()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { "Hello World!", "Good day.", "Bye bye." });

        int messageCount = 0;
        _pubsubFixture.Pull.Eventually(() =>
        {
            // Pull and acknowledge the messages
            messageCount += _pullMessageWithLeaseManagementSample.PullMessageWithLeaseManagement(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(3, messageCount);
        });
    }
}
