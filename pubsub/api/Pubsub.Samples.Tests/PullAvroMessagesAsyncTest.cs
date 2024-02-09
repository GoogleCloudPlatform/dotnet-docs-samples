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
public class PullAvroMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishAvroMessagesAsyncSample _publishAvroMessagesAsyncSample;
    private readonly PullAvroMessagesAsyncSample _pullAvroMessagesAsyncSample;

    public PullAvroMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishAvroMessagesAsyncSample = new PublishAvroMessagesAsyncSample();
        _pullAvroMessagesAsyncSample = new PullAvroMessagesAsyncSample();
    }

    [Fact]
    public async Task PullAvroBinaryMessagesAsync()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Binary);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishAvroMessagesAsyncSample.PublishAvroMessagesAsync(_pubsubFixture.ProjectId, topicId, new AvroUtilities.State[] { new AvroUtilities.State { name = "New York", post_abbr = "NY" } });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var result = await _pullAvroMessagesAsyncSample.PullAvroMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, result);
        });

        //Pull the Message to confirm it's gone after it's acknowledged
        var result = await _pullAvroMessagesAsyncSample.PullAvroMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task PullAvroJsonMessagesAsync()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        _pubsubFixture.CreateAvroSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schemaId, Encoding.Json);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        await _publishAvroMessagesAsyncSample.PublishAvroMessagesAsync(_pubsubFixture.ProjectId, topicId, new AvroUtilities.State[] { new AvroUtilities.State { name = "New York", post_abbr = "NY" } });

        await _pubsubFixture.Pull.Eventually(async () =>
        {
            // Pull and acknowledge the messages
            var result = await _pullAvroMessagesAsyncSample.PullAvroMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
            Assert.Equal(1, result);
        });

        //Pull the Message to confirm it's gone after it's acknowledged
        var result = await _pullAvroMessagesAsyncSample.PullAvroMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, true);
        Assert.Equal(0, result);
    }
}
