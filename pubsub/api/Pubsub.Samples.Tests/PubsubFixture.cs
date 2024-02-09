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

using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.PubSub.V1;
using Google.Cloud.Storage.V1;
using GoogleCloudSamples;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

[CollectionDefinition(nameof(PubsubFixture))]
public class PubsubFixture : IDisposable, ICollectionFixture<PubsubFixture>
{
    public string ProjectId { get; }
    public List<string> TempTopicIds { get; } = new List<string>();
    public List<string> TempSubscriptionIds { get; } = new List<string>();
    public List<string> TempSchemaIds { get; } = new List<string>();
    public string DeadLetterTopic { get; } = $"testDeadLetterTopic{Guid.NewGuid().ToString().Substring(0, 18)}";
    public string AvroSchemaFile { get; } = "Resources/us-states.avsc";
    // The schema to use when to update an Avro schema
    public string NewAvroSchemaFile { get; } = "Resources/us-states-new.avsc";
    public string ProtoSchemaFile { get; } = "Resources/us-states.proto";
    // The schema to use when to update a proto schema
    public string NewProtoSchemaFile { get; } = "Resources/us-states-new.proto";
    public string BigQueryDatasetId { get; } = $"testDataSet{Guid.NewGuid().ToString().Substring(24)}";
    public string BigQueryTableId { get; } = $"testTable{Guid.NewGuid().ToString().Substring(24)}";
    public string BigQueryTableName { get; }
    public string CloudStorageBucketName { get; }

    public RetryRobot Pull { get; } = new RetryRobot
    {
        ShouldRetry = ex => ex is XunitException
            || (ex is RpcException rpcEx
                && (rpcEx.StatusCode == StatusCode.DeadlineExceeded || rpcEx.StatusCode == StatusCode.Unavailable))
    };

    public PubsubFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        CreateTopic(DeadLetterTopic);
        BigQueryTableName = CreateBigQueryTable();
        CloudStorageBucketName = CreateCloudStorageBucket();
    }

    public void Dispose()
    {
        var deleteTopicSampleObject = new DeleteTopicSample();
        var deleteSubscriptionSampleObject = new DeleteSubscriptionSample();
        var deleteSchemaSampleObject = new DeleteSchemaSample();
        foreach (string subscriptionId in TempSubscriptionIds)
        {
            try
            {
                deleteSubscriptionSampleObject.DeleteSubscription(ProjectId, subscriptionId);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Exception occur while deleting subscription {subscriptionId} Exception: {ex}");
            }
        }
        foreach (string topicId in TempTopicIds)
        {
            try
            {
                deleteTopicSampleObject.DeleteTopic(ProjectId, topicId);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Exception occur while deleting Topic {topicId} Exception: {ex}");
            }
        }
        foreach (string schemaId in TempSchemaIds)
        {
            try
            {
                deleteSchemaSampleObject.DeleteSchema(ProjectId, schemaId);
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Exception occur while deleting Schema {schemaId} Exception: {ex}");
            }
        }
        DeleteBigQueryTable();
        DeleteCloudStorageBucket();
    }

    public Topic CreateTopic(string topicId)
    {
        var createTopicSampleObject = new CreateTopicSample();
        TempTopicIds.Add(topicId);
        return createTopicSampleObject.CreateTopic(ProjectId, topicId);
    }

    public Topic CreateTopicWithSchema(string topicId, string schemaId, Encoding encoding)
    {
        var createTopicWithSchemaSampleObject = new CreateTopicWithSchemaSample();
        var topic = createTopicWithSchemaSampleObject.CreateTopicWithSchema(ProjectId, topicId, schemaId, encoding);
        TempTopicIds.Add(topicId);
        return topic;
    }

    public Subscription CreateSubscription(string topicId, string subscriptionId)
    {
        var createSubscriptionSampleObject = new CreateSubscriptionSample();
        var subscription = createSubscriptionSampleObject.CreateSubscription(ProjectId, topicId, subscriptionId);
        TempSubscriptionIds.Add(subscriptionId);
        return subscription;
    }

    public Subscription CreateExactlyOnceDeliverySubscription(string topicId, string subscriptionId)
    {
        var createSubscriptionWithExactlyOnceDeliverySample = new CreateSubscriptionWithExactlyOnceDeliverySample();
        TempSubscriptionIds.Add(subscriptionId);
        return createSubscriptionWithExactlyOnceDeliverySample.CreateSubscriptionWithExactlyOnceDelivery(ProjectId, topicId, subscriptionId);
    }

    public Schema CreateProtoSchema(string schemaId)
    {
        var createProtoSchemaSampleObject = new CreateProtoSchemaSample();
        var schema = createProtoSchemaSampleObject.CreateProtoSchema(ProjectId, schemaId, ProtoSchemaFile);
        TempSchemaIds.Add(schemaId);
        return schema;
    }

    public Schema CreateAvroSchema(string schemaId)
    {
        var createAvroSchemaSampleObject = new CreateAvroSchemaSample();
        var schema = createAvroSchemaSampleObject.CreateAvroSchema(ProjectId, schemaId, AvroSchemaFile);
        TempSchemaIds.Add(schemaId);
        return schema;
    }

    public string CreateBigQueryTable()
    {
        BigQueryClient client = BigQueryClient.Create(ProjectId);
        var dataset = new Dataset
        {
            // Specify the geographic location where the dataset should reside.
            Location = "US"
        };
        // Create the dataset
        var createdDataset = client.CreateDataset(datasetId: BigQueryDatasetId, dataset);

        // Create schema for new table.
        var schema = new TableSchemaBuilder
        {
            { "data", BigQueryDbType.Bytes },
            { "message_id", BigQueryDbType.String },
            { "subscription_name", BigQueryDbType.String },
            { "attributes", BigQueryDbType.String },
            { "publish_time", BigQueryDbType.Timestamp }
        }.Build();
        // Create the table
        createdDataset.CreateTable(tableId: BigQueryTableId, schema: schema);

        return $"{ProjectId}.{BigQueryDatasetId}.{BigQueryTableId}";
    }

    public string CreateCloudStorageBucket()
    {
        StorageClient client = StorageClient.Create();
        Bucket bucket = client.CreateBucket(ProjectId, RandomName("testbucket-"));
        return bucket.Name;
    }

    public void DeleteBigQueryTable()
    {
        BigQueryClient client = BigQueryClient.Create(ProjectId);
        DeleteDatasetOptions options = new DeleteDatasetOptions
        {
            DeleteContents = true
        };
        client.DeleteDataset(datasetId: BigQueryDatasetId, options);
    }

    public void DeleteCloudStorageBucket()
    {
        StorageClient client = StorageClient.Create();
        DeleteBucketOptions options = new DeleteBucketOptions
        {
            DeleteObjects = true
        };
        client.DeleteBucket(CloudStorageBucketName, options);
    }

    public Topic GetTopic(string topicId)
    {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        TopicName topicName = TopicName.FromProjectTopic(ProjectId, topicId);
        return publisher.GetTopic(topicName);
    }

    public Subscription GetSubscription(string subscriptionId)
    {
        SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
        SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(ProjectId, subscriptionId);

        return subscriber.GetSubscription(subscriptionName);
    }

    public Schema GetSchema(string schemaId)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        SchemaName schemaName = SchemaName.FromProjectSchema(ProjectId, schemaId);
        GetSchemaRequest request = new GetSchemaRequest
        {
            Name = schemaName.ToString(),
            View = SchemaView.Full
        };

        return schemaService.GetSchema(request);
    }

    public List<Schema> ListSchemaRevisions(string schemaId)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        SchemaName schemaName = SchemaName.FromProjectSchema(ProjectId, schemaId);
        return schemaService.ListSchemaRevisions(schemaName).ToList();
    }

    public string RandomName() => Guid.NewGuid().ToString().Substring(0, 18);

    public string RandomName(string prefix) => prefix + RandomName();

    public string RandomSchemaId([CallerMemberName] string caller = null) => RandomName($"test{caller}Schema");

    public string RandomTopicId([CallerMemberName] string caller = null) => RandomName($"test{caller}Topic");

    public (string topicId, string subscriptionId) RandomNameTopicSubscriptionId([CallerMemberName] string caller = null)
    {
        var randomName = RandomName();
        return ($"test{caller}Topic{randomName}", $"test{caller}Subscription{randomName}");
    }

    public (string topicId, string subscriptionId, string schemaId) RandomNameTopicSubscriptionSchemaId([CallerMemberName] string caller = null)
    {
        var randomName = RandomName();
        return ($"test{caller}Topic{randomName}", $"test{caller}Subscription{randomName}", $"test{caller}Schema{randomName}");
    }
}
