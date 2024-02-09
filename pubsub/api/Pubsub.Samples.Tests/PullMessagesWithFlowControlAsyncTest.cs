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
public class PullMessagesWithFlowControlAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishMessagesAsyncSample _publishMessagesAsyncSample;
    private readonly PullMessagesWithFlowControlAsyncSample _pullMessagesCustomAsyncSample;

    public PullMessagesWithFlowControlAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishMessagesAsyncSample = new PublishMessagesAsyncSample();
        _pullMessagesCustomAsyncSample = new PullMessagesWithFlowControlAsyncSample();
    }

    [Fact]
    public async Task PullMessagesWithFlowControlAsync()
    {
        string topicId = _pubsubFixture.RandomTopicId();
        _pubsubFixture.CreateTopic(topicId);

        // For this sample in particular, we need to retry the sbscription creation, the message publishing etc.
        // That's because this sample is configuring the ack deadline of the subscriber, which will be extended
        // automatically. While Pub/Sub has sent a message to a subscriber it will avoid sending it to another subscriber
        // of the same subscription until the ack deadline has expired. Since we are renewing the ack deadline, it won't
        // expire (soon) and if the message is sent but not acked, then Pub/Sub won't attempt to send the message
        // to a different susbcriber even if we keep retrying, as it is waiting for the ack deadline to expire, which we
        // keep extending.
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            string subscriptionId = _pubsubFixture.RandomName("testSubscriptionForMessageWithFlowControlAck");
            _pubsubFixture.CreateSubscription(topicId, subscriptionId);

            var message = _pubsubFixture.RandomName();
            await _publishMessagesAsyncSample.PublishMessagesAsync(_pubsubFixture.ProjectId, topicId, new string[] { message });

            // Pull and acknowledge the messages
            var result = await _pullMessagesCustomAsyncSample.PullMessagesWithFlowControlAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, result);

            //Pull the Message to confirm it's gone after it's acknowledged
            result = await _pullMessagesCustomAsyncSample.PullMessagesWithFlowControlAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(0, result);
        });
    }
}
