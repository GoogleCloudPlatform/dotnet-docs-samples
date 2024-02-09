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

using System;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class CreateCloudStorageSubscriptionTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CreateCloudStorageSubscriptionSample _createCloudStorageSubscriptionSample;

    public CreateCloudStorageSubscriptionTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _createCloudStorageSubscriptionSample = new CreateCloudStorageSubscriptionSample();
    }

    [Fact]
    public void CreateCloudStorageSubscription()
    {
        var (topicId, subscriptionId) = _pubsubFixture.RandomNameTopicSubscriptionId();
        string bucket = _pubsubFixture.CloudStorageBucketName;

        _pubsubFixture.CreateTopic(topicId);

        _pubsubFixture.TempSubscriptionIds.Add(subscriptionId);
        var subscription = _createCloudStorageSubscriptionSample.CreateCloudStorageSubscription(
            _pubsubFixture.ProjectId, topicId, subscriptionId, bucket, "prefix", "suffix", TimeSpan.FromMinutes(5));

        Assert.Equal(subscription.CloudStorageConfig.Bucket, bucket);
    }
}
