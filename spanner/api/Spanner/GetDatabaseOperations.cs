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

// [START spanner_get_database_operations]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class GetDatabaseOperations
    {
        public static object SpannerGetDatabaseOperations(string projectId, string instanceId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            var filter = "(metadata.@type:type.googleapis.com/google.spanner.admin.database.v1.OptimizeRestoredDatabaseMetadata)";

            ListDatabaseOperationsRequest request = new ListDatabaseOperationsRequest
            {
                ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
                Filter = filter
            };

            // List the optimize restored databases operations on the instance.
            var operations = databaseAdminClient.ListDatabaseOperations(request);

            foreach (var operation in operations)
            {
                OptimizeRestoredDatabaseMetadata metadata =
                    operation.Metadata.Unpack<OptimizeRestoredDatabaseMetadata>();
                Console.WriteLine(
                    $"Database {metadata.Name} restored from backup is " +
                    $"{metadata.Progress.ProgressPercent}% optimized");
            }

            return 0;
        }
    }
}
// [END spanner_get_database_operations]
