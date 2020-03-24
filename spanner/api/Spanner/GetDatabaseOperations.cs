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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloudSamples.Spanner
{
    public class GetDatabaseOperations
    {
        // [START spanner_get_database_operations]
        public static List<Operation> SpannerGetDatabaseOperations(string projectId, string instanceId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            var filter = "(metadata.@type:type.googleapis.com/google.spanner.admin.database.v1.OptimizeRestoredDatabaseMetadata)";

            ListDatabaseOperationsRequest listDatabaseOperationsRequest = new ListDatabaseOperationsRequest
            {
                Filter = filter,
                Parent = InstanceName.Format(projectId, instanceId).ToString()
            };

            // Make the ListDatabaseOperations request
            var operations = databaseAdminClient.ListDatabaseOperations(listDatabaseOperationsRequest).ToList();
            return operations;
        }
        // [END spanner_get_database_operations]
    }
}
