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

using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class RpcRetryTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly GetTopicSample _getTopicSample;
    public RpcRetryTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _getTopicSample = new GetTopicSample();
    }

    [Fact]
    public void TestRpcRetry()
    {
        string topicId = "testTopicForRpcRetry" + _pubsubFixture.RandomName();
        string subscriptionId = "testSubscriptionForRpcRetry" + _pubsubFixture.RandomName();

        _pubsubFixture.TempTopicIds.Add(topicId);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        _pubsubFixture.Eventually(() =>
        {
            RpcRetry(topicId, subscriptionId, _pubsubFixture.Publisher, _pubsubFixture.Subscriber);
            var topicDetails = _getTopicSample.GetTopic(_pubsubFixture.ProjectId, topicId);
            Assert.Equal(topicId, topicDetails.TopicName.TopicId);
        });
    }

    internal void RpcRetry(string topicId, string subscriptionId,
            PublisherServiceApiClient publisher, SubscriberServiceApiClient subscriber)
    {
        TopicName topicName = new TopicName(_pubsubFixture.ProjectId, topicId);
        // Create Subscription.
        SubscriptionName subscriptionName = new SubscriptionName(_pubsubFixture.ProjectId,
            subscriptionId);
        // Create Topic
        try
        {
            // This may fail if the Topic already exists.
            // Don't retry in that case.
            publisher.CreateTopic(topicName, _pubsubFixture.NewRetryCallSettings(3,
                StatusCode.AlreadyExists));
        }
        catch (RpcException e)
        when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }
        try
        {
            // Subscribe to Topic
            // This may fail if the Subscription already exists.  Don't
            // retry, because a retry would fail the same way.
            subscriber.CreateSubscription(subscriptionName, topicName,
                pushConfig: null, ackDeadlineSeconds: 60,
                callSettings: _pubsubFixture.NewRetryCallSettings(3,
                    StatusCode.AlreadyExists));
        }
        catch (RpcException e)
        when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }
    }
}
