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

using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class DeleteSubscriptionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly DeleteSubscriptionSample _deleteSubscriptionSample;

    public DeleteSubscriptionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _deleteSubscriptionSample = new DeleteSubscriptionSample();
    }

    [Fact]
    public void DeleteSubscription()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();

        _pubsubFixture.CreateTopic(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        _deleteSubscriptionSample.DeleteSubscription(_pubsubFixture.ProjectId, subscriptionId);

        Exception e = Assert.Throws<Grpc.Core.RpcException>(() => _pubsubFixture.GetSubscription(subscriptionId));

        _pubsubFixture.TempSubscriptionIds.Remove(subscriptionId);  // We already deleted it.
    }
}
