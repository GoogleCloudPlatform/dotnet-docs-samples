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

using Avro;
using Avro.Generic;
using Avro.IO;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using System.IO;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class SubscribeAvroRecordsWithRevisionsTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly CommitAvroSchemaSample _commitAvroSchemaSample;
    private readonly SubscribeAvroRecordsWithRevisionsSample _subscribeAvroRecordsWithRevisionsSample;

    public SubscribeAvroRecordsWithRevisionsTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _commitAvroSchemaSample = new CommitAvroSchemaSample();
        _subscribeAvroRecordsWithRevisionsSample = new SubscribeAvroRecordsWithRevisionsSample();
    }

    [Fact]
    public async Task SubscribeAvroRecordsWithRevisions()
    {
        var (topicId, subscriptionId, schemaId) = _pubsubFixture.RandomNameTopicSubscriptionSchemaId();

        var initialPubSubSchema = _pubsubFixture.CreateAvroSchema(schemaId);
        var amendedPubSubSchema = _commitAvroSchemaSample.CommitAvroSchema(_pubsubFixture.ProjectId, schemaId, _pubsubFixture.NewAvroSchemaFile);

        // Create the topic and publish the messages directly, as that's simpler than going through other samples.
        var publisherClient = PublisherServiceApiClient.Create();
        var topic = new Topic
        {
            TopicName = new TopicName(_pubsubFixture.ProjectId, topicId),
            SchemaSettings = new SchemaSettings
            {
                SchemaAsSchemaName = new Google.Cloud.PubSub.V1.SchemaName(_pubsubFixture.ProjectId, schemaId),
                FirstRevisionId = initialPubSubSchema.RevisionId,
                LastRevisionId = amendedPubSubSchema.RevisionId,
                Encoding = Encoding.Binary
            }
        };

        publisherClient.CreateTopic(topic);
        _pubsubFixture.TempTopicIds.Add(topicId);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        var schema1 = (RecordSchema) Avro.Schema.Parse(initialPubSubSchema.Definition);
        var schema2 = (RecordSchema) Avro.Schema.Parse(amendedPubSubSchema.Definition);
        var messages = new[]
        {
            CreateMessage("New York", "NY", null, schema1),
            CreateMessage("Califonia", "CA", null, schema1),
            CreateMessage("Washington", "WA", "Olympia", schema2),
        };
        publisherClient.Publish(topic.TopicName, messages);

        var (messageCount, schemaCount) = await _subscribeAvroRecordsWithRevisionsSample.SubscribeAvroRecordsWithRevisions(_pubsubFixture.ProjectId, subscriptionId);

        Assert.Equal(3, messageCount);
        Assert.Equal(2, schemaCount);
        
        PubsubMessage CreateMessage(string name, string abbr, string capital, RecordSchema schema)
        {
            var record = new GenericRecord(schema);
            record.Add(0, name);
            record.Add(1, abbr);
            if (capital != null)
            {
                record.Add(2, capital);
            }
            var ms = new MemoryStream();
            var encoder = new BinaryEncoder(ms);
            new DefaultWriter(schema).Write(record, encoder);
            return new PubsubMessage { Data = ByteString.CopyFrom(ms.ToArray()) };
        }
    }
}
