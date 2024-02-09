// Copyright 2024 Google Inc.
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
public class CreateUnwrappedPushSubscriptionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateUnwrappedPushSubscriptionSample _createUnwrappedPushSubscriptionSample;

    public CreateUnwrappedPushSubscriptionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createUnwrappedPushSubscriptionSample = new CreateUnwrappedPushSubscriptionSample();
    }

    [Fact]
    public void CreateUnwrappedPushSubscription()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        string pushEndpoint = "https://my-test-project.appspot.com/push";

        _pubsubFixture.CreateTopic(topicId);

        var subscription = _createUnwrappedPushSubscriptionSample.CreateUnwrappedPushSubscription(_pubsubFixture.ProjectId, topicId, subscriptionId, pushEndpoint);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        Assert.Equal(subscription.PushConfig.PushEndpoint, pushEndpoint);
    }
}
