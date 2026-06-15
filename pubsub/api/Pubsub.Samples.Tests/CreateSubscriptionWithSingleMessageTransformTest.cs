// Copyright 2026 Google LLC.
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
public class CreateSubscriptionWithSingleMessageTransformTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateSubscriptionWithSingleMessageTransformSample _createSubscriptionSample;

    public CreateSubscriptionWithSingleMessageTransformTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createSubscriptionSample = new CreateSubscriptionWithSingleMessageTransformSample();
    }

    [Fact]
    public void CreateSubscriptionWithSingleMessageTransform()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        _pubsubFixture.TempTopicIds.Add(topicId);
        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);

        _pubsubFixture.CreateTopic(topicId);

        _createSubscriptionSample.CreateSubscriptionWithSingleMessageTransform(_pubsubFixture.ProjectId, topicId, subscriptionId);
        var subscription = _pubsubFixture.GetSubscription(subscriptionId);
        Assert.Equal("redactSsn", subscription.MessageTransforms[0].JavascriptUdf.FunctionName);
    }
}
