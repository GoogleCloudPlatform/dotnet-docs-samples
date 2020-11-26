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

// [START datastore_admin_index_get]

using Google.Cloud.Datastore.Admin.V1;
using System;
using Index = Google.Cloud.Datastore.Admin.V1.Index;

public class GetIndexSample
{
    public Index GetIndex(
        string projectId = "your-project-id",
        string indexId = "your-index-id")
    {
        // Create client
        DatastoreAdminClient datastoreAdminClient = DatastoreAdminClient.Create();

        // Initialize request argument(s)
        GetIndexRequest getIndexRequest = new GetIndexRequest
        {
            ProjectId = projectId,
            IndexId = indexId
        };

        Index index = datastoreAdminClient.GetIndex(getIndexRequest);

        Console.WriteLine($"Index Id: {index.IndexId}");
        Console.WriteLine($"Kind: {index.Kind}");
        Console.WriteLine("Properties:");
        foreach (var property in index.Properties)
        {
            Console.WriteLine($"Property: {property.Name}");
            Console.WriteLine($"Direction: {property.Direction}");
        }
        return index;
    }
}
// [END datastore_admin_index_get]
