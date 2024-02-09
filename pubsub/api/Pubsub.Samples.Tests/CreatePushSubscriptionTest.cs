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
public class CreatePushSubscriptionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreatePushSubscriptionSample _createPushSubscriptionSample;

    public CreatePushSubscriptionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createPushSubscriptionSample = new CreatePushSubscriptionSample();
    }

    [Fact]
    public void CreatePushSubscription()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        string pushEndpoint = "https://my-test-project.appspot.com/push";

        _pubsubFixture.CreateTopic(topicId);

        var subscription = _createPushSubscriptionSample.CreatePushSubscription(_pubsubFixture.ProjectId, topicId, subscriptionId, pushEndpoint);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        Assert.Equal(subscription.PushConfig.PushEndpoint, pushEndpoint);
    }
}
