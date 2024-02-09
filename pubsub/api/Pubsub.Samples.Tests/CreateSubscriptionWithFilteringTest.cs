// Copyright 2022 Google Inc.
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
public class CreateSubscriptionWithFilteringTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateSubscriptionWithFilteringSample _createSubscriptionWithFilteringSample;

    public CreateSubscriptionWithFilteringTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createSubscriptionWithFilteringSample = new CreateSubscriptionWithFilteringSample();
    }

    [Fact]
    public void CreateSubscriptionWithFiltering()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        string filter = "attributes:domain";

        _pubsubFixture.CreateTopic(topicId);
        _createSubscriptionWithFilteringSample.CreateSubscriptionWithFiltering(_pubsubFixture.ProjectId, topicId, subscriptionId, filter);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);
        var subscription = _pubsubFixture.GetSubscription(subscriptionId);

        Assert.Equal("attributes:domain", subscription.Filter);
    }
}
