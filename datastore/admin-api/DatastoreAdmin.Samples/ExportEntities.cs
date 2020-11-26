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

// [START datastore_admin_entities_export]

using Google.Cloud.Datastore.Admin.V1;
using System;
using System.Collections.Generic;

public class ExportEntitiesSample
{
    public string ExportEntities(
        string projectId = "your-project-id",
        string outputUrlPrefix = "gs://your-bucket-name",
        string kind = "Task",
        string namespaceId = "default")
    {
        // [START datastore_admin_client_create]
        // Create client
        DatastoreAdminClient datastoreAdminClient = DatastoreAdminClient.Create();
        // [END datastore_admin_client_create]

        IDictionary<string, string> labels = new Dictionary<string, string> { { "cloud_datastore_samples", "true" }, };
        EntityFilter entityFilter = new EntityFilter
        {
            Kinds = { kind },
            NamespaceIds = { namespaceId }
        };

        var response = datastoreAdminClient.ExportEntities(projectId, labels, entityFilter, outputUrlPrefix);

        // Poll until the returned long-running operation is complete
        var completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while Exporting Entities: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Entities exported successfully.");

        ExportEntitiesResponse result = completedResponse.Result;

        return result.OutputUrl;
    }
}
// [END datastore_admin_entities_export]
