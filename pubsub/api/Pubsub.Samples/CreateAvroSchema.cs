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

// [START pubsub_create_avro_schema]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.IO;

public class CreateAvroSchemaSample
{
    public Schema CreateAvroSchema(string projectId, string schemaId, string pathToDefinition)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        var schemaName = SchemaName.FromProjectSchema(projectId, schemaId);
        string schemaDefinition = File.ReadAllText(pathToDefinition);
        Schema schema = new Schema
        {
            SchemaName = schemaName,
            Type = Schema.Types.Type.Avro,
            Definition = schemaDefinition
        };
        CreateSchemaRequest createSchemaRequest = new CreateSchemaRequest
        {
            ParentAsProjectName = ProjectName.FromProject(projectId),
            SchemaId = schemaId,
            Schema = schema
        };

        try
        {
            schema = schemaService.CreateSchema(createSchemaRequest);
            Console.WriteLine($"Schema {schema.Name} created.");
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            Console.WriteLine($"Schema {schemaName} already exists.");
        }
        return schema;
    }
}
// [END pubsub_create_avro_schema]
