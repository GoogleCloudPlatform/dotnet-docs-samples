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

// [START datastore_admin_index_list]

using Google.Cloud.Datastore.Admin.V1;
using System;
using System.Collections.Generic;

public class ListIndexesSample
{
    public IEnumerable<Google.Cloud.Datastore.Admin.V1.Index> ListIndexes(string projectId = "your-project-id")
    {
        // Create client
        DatastoreAdminClient datastoreAdminClient = DatastoreAdminClient.Create();

        // Initialize request argument(s)
        ListIndexesRequest listIndexesRequest = new ListIndexesRequest
        {
            ProjectId = projectId
        };

        var response = datastoreAdminClient.ListIndexes(listIndexesRequest);

        foreach (var index in response)
        {
            Console.WriteLine($"Index Id: {index.IndexId}");
            Console.WriteLine($"Kind: {index.Kind}");

            Console.WriteLine("Properties:");
            foreach (var property in index.Properties)
            {
                Console.WriteLine($"Property: {property.Name}");
                Console.WriteLine($"Direction: {property.Direction}");
            }
        }

        return response;
    }
}
// [END datastore_admin_index_list]
