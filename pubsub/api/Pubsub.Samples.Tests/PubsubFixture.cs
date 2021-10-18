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

using Google.Cloud.PubSub.V1;
using GoogleCloudSamples;
using Grpc.Core;
using System;
using System.Collections.Generic;
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
    public string AvroSchemaFile { get; } = $"Resources/us-states.avsc";
    public string ProtoSchemaFile { get; } = $"Resources/us-states.proto";

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
            catch (RpcException)
            {
                // Do nothing, we are deleting on a best effort basis.
            }
        }
        foreach (string topicId in TempTopicIds)
        {
            try
            {
                deleteTopicSampleObject.DeleteTopic(ProjectId, topicId);
            }
            catch (RpcException)
            {
                // Do nothing, we are deleting on a best effort basis.
            }
        }
        foreach (string schemaId in TempSchemaIds)
        {
            try
            {
                deleteSchemaSampleObject.DeleteSchema(ProjectId, schemaId);
            }
            catch (RpcException)
            {
                // Do nothing, we are deleting on a best effort basis.
            }
        }
    }

    public Topic CreateTopic(string topicId)
    {
        var createTopicSampleObject = new CreateTopicSample();
        var topic = createTopicSampleObject.CreateTopic(ProjectId, topicId);
        TempTopicIds.Add(topicId);
        return topic;
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

    public string RandomName()
    {
        return Guid.NewGuid().ToString().Substring(0, 18);
    }
}
