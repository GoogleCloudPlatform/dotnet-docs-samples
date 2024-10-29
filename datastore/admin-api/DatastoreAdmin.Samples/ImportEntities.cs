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

// [START datastore_admin_entities_import]

using Google.Cloud.Datastore.Admin.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class ImportEntitiesSample
{
    public bool ImportEntities(
        string projectId = "your-project-id",
        // For information on accepted input URL formats, see
        // https://cloud.google.com/datastore/docs/reference/admin/rpc/google.datastore.admin.v1#google.datastore.admin.v1.ImportEntitiesRequest
        string inputUrl = "[URL to file containing data]",
        string kind = "Task",
        string namespaceId = "default")
    {
        // Create client
        DatastoreAdminClient datastoreAdminClient = DatastoreAdminClient.Create();

        IDictionary<string, string> labels = new Dictionary<string, string> { { "cloud_datastore_samples", "true" }, };
        EntityFilter entityFilter = new EntityFilter()
        {
            Kinds = { kind },
            NamespaceIds = { namespaceId }
        };

        Operation<Empty, ImportEntitiesMetadata> response = datastoreAdminClient.ImportEntities(projectId, labels, inputUrl, entityFilter);

        // Poll until the returned long-running operation is complete
        Operation<Empty, ImportEntitiesMetadata> completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while Importing Entities: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Entities imported successfully.");

        return completedResponse.IsCompleted;
    }
}
// [END datastore_admin_entities_import]
