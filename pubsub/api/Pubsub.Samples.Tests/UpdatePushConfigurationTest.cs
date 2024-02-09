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
public class UpdatePushConfigurationTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreatePushSubscriptionSample _createPushSubscriptionSample;
    private readonly UpdatePushConfigurationSample _updatePushConfigurationSample;
    private readonly ListSubscriptionsSample _listSubscriptionsSample;

    public UpdatePushConfigurationTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createPushSubscriptionSample = new CreatePushSubscriptionSample();
        _updatePushConfigurationSample = new UpdatePushConfigurationSample();
        _listSubscriptionsSample = new ListSubscriptionsSample();
    }

    [Fact]
    public void UpdatePushConfiguration()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        _pubsubFixture.CreateTopic(topicId);
        _createPushSubscriptionSample.CreatePushSubscription(_pubsubFixture.ProjectId, topicId, subscriptionId, "https://my-test-project.appspot.com/push");
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        var updatedPushEndpoint = "https://my-test-project.appspot.update.com/push";
        _updatePushConfigurationSample.UpdatePushConfiguration(_pubsubFixture.ProjectId, subscriptionId, updatedPushEndpoint);

        var subscriptions = _listSubscriptionsSample.ListSubscriptions(_pubsubFixture.ProjectId);
        Assert.Contains(subscriptions, s => s.PushConfig.PushEndpoint == updatedPushEndpoint && s.SubscriptionName.SubscriptionId == subscriptionId);
    }
}
