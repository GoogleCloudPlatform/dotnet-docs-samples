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

// [START pubsub_commit_avro_schema]

using Google.Cloud.PubSub.V1;
using System;
using System.IO;

public class CommitAvroSchemaSample
{
    public Schema CommitAvroSchema(string projectId, string schemaId, string pathToNewDefinition)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        var schemaName = SchemaName.FromProjectSchema(projectId, schemaId);
        string schemaDefinition = File.ReadAllText(pathToNewDefinition);
        Schema schema = new Schema
        {
            SchemaName = schemaName,
            Type = Schema.Types.Type.Avro,
            Definition = schemaDefinition
        };

        schema = schemaService.CommitSchema(schemaName, schema);
        Console.WriteLine($"Schema {schema.Name} committed.");
        return schema;
    }
}
// [END pubsub_commit_avro_schema]
