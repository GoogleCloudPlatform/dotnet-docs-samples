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

// [START pubsub_get_schema]

using Google.Cloud.PubSub.V1;

public class GetSchemaSample
{
    public Schema GetSchema(string projectId, string schemaId)
    {
        SchemaServiceClient schemaService = SchemaServiceClient.Create();
        SchemaName schemaName = SchemaName.FromProjectSchema(projectId, schemaId);
        GetSchemaRequest request = new GetSchemaRequest
        {
            Name = schemaName.ToString(),
            View = SchemaView.Full
        };

        return schemaService.GetSchema(request);
    }
}
// [END pubsub_get_schema]
