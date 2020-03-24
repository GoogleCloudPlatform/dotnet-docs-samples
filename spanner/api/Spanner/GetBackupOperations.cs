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
    public class GetBackupOperations
    {
        // [START spanner_get_backup_operations]
        public static List<Operation> SpannerGetBackupOperations(string projectId, string instanceId, string databaseId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            var filter = $"(metadata.database:{databaseId}) AND (metadata.@type:type.googleapis.com/google.spanner.admin.database.v1.CreateBackupMetadata)";

            // Make the List backup operation request instance.
            ListBackupOperationsRequest request = new ListBackupOperationsRequest
            {
                Parent = InstanceName.Format(projectId, instanceId),
                Filter = filter
            };

            // List the create backup operations on the database.
            var backupOperations = databaseAdminClient.ListBackupOperations(request).ToList();
            return backupOperations;
        }
        // [END spanner_get_backup_operations]
    }
}
