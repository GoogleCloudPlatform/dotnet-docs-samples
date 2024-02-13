// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
//     https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START pubsub_subscribe_avro_records_with_revisions]
using Avro.Generic;
using Avro.IO;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class SubscribeAvroRecordsWithRevisionsSample
{
    public async Task<(int, int)> SubscribeAvroRecordsWithRevisions(string projectId, string subscriptionId)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();

        var schemaCache = new ConcurrentDictionary<(string, string), Avro.Schema>();

        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
        SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
        int messageCount = 0;
        Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            // Get the schema name, revision ID and encoding type from the message.
            string encoding = message.Attributes["googclient_schemaencoding"];
            string schemaName = message.Attributes["googclient_schemaname"];
            string revision = message.Attributes["googclient_schemarevisionid"];

            // Fetch the schema if we don't already have it.
            var avroSchema = schemaCache.GetOrAdd((schemaName, revision), key =>
            {
                var pubSubSchema = schemaService.GetSchema($"{schemaName}@{revision}");
                return Avro.Schema.Parse(pubSubSchema.Definition);
            });

            // Read the message.
            if (encoding == "BINARY")
            {
                using var ms = new MemoryStream(message.Data.ToByteArray());
                var decoder = new BinaryDecoder(ms);
                var reader = new DefaultReader(avroSchema, avroSchema);
                var record = reader.Read<GenericRecord>(null, decoder);
                Console.WriteLine($"Message {message.MessageId}: {record.GetValue(0)}");
                Interlocked.Increment(ref messageCount);
            }
            else
            {
                Console.WriteLine("Expected only binary messages in this sample");
            }
            return Task.FromResult(SubscriberClient.Reply.Ack);
        });
        // Run for 10 seconds.
        await Task.Delay(10_000);
        await subscriber.StopAsync(CancellationToken.None);
        // Lets make sure that the start task finished successfully after the call to stop.
        await startTask;
        return (messageCount, schemaCache.Count);
    }
}

// [END pubsub_subscribe_avro_records_with_revisions]
