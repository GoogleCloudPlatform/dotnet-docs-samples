// Copyright 2021 Google Inc.
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
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PullProtoMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishProtoMessagesAsyncSample _publishProtoMessagesAsyncSample;
    private readonly PullProtoMessagesAsyncSample _pullProtoMessagesAsyncSample;

    public PullProtoMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishProtoMessagesAsyncSample = new PublishProtoMessagesAsyncSample();
        _pullProtoMessagesAsyncSample = new PullProtoMessagesAsyncSample();
    }

    [Fact]
    public async Task PullProtoBinaryMessagesAsync()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateProtoSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Binary);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishProtoMessagesAsyncSample.PublishProtoMessagesAsync(_pubsubFixture.ProjectId, topicId, new Utilities.State[] { new Utilities.State { Name = "New York", PostAbbr = "NY" } });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var ackedCount = await _pullProtoMessagesAsyncSample.PullProtoMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, ackedCount);
        });

        //Pull the Message to confirm it's gone after it's acknowledged
        var result = await _pullProtoMessagesAsyncSample.PullProtoMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task PullProtoJsonMessagesAsync()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateProtoSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Json);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishProtoMessagesAsyncSample.PublishProtoMessagesAsync(_pubsubFixture.ProjectId, topicId, new Utilities.State[] { new Utilities.State { Name = "New York", PostAbbr = "NY" } });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var ackedCount = await _pullProtoMessagesAsyncSample.PullProtoMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, ackedCount);
        });

        //Pull the Message to confirm it's gone after it's acknowledged
        var result = await _pullProtoMessagesAsyncSample.PullProtoMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }
}
