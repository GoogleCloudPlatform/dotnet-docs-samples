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
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishAvroMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishAvroMessagesAsyncSample _publishAvroMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PublishAvroMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishAvroMessagesAsyncSample = new PublishAvroMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async Task PublishBinaryMessages()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Binary);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        List<AvroUtilities.State> messageTexts = new List<AvroUtilities.State> { new AvroUtilities.State { name = "New York", post_abbr = "NY" }, new AvroUtilities.State { name = "Pennsylvania", post_abbr = "PA" } };

        var output = await _publishAvroMessagesAsyncSample.PublishAvroMessagesAsync(_pubsubFixture.ProjectId, topicId, messageTexts);
        Assert.Equal(messageTexts.Count, output);

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
            Assert.True(result > 0);
        });
    }

    [Fact]
    public async Task PublishJsonMessages()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Json);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        List<AvroUtilities.State> messageTexts = new List<AvroUtilities.State> { new AvroUtilities.State { name = "New York", post_abbr = "NY" }, new AvroUtilities.State { name = "Pennsylvania", post_abbr = "PA" } };

        var output = await _publishAvroMessagesAsyncSample.PublishAvroMessagesAsync(_pubsubFixture.ProjectId, topicId, messageTexts);
        Assert.Equal(messageTexts.Count, output);

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
            Assert.True(result > 0);
        });
    }
}
