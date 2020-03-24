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
using Google.LongRunning;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class RestoreDatabase
    {
        // [START spanner_restore_database]
        public static RestoreInfo SpannerRestoreDatabase(string backupId, string destinationInstanceId,
            string destinationDatabaseId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            RestoreDatabaseRequest restoreRequest = new RestoreDatabaseRequest
            {
                Parent = destinationInstanceId,
                Backup = backupId,
                DatabaseId = destinationDatabaseId,
            };

            // Make the RestoreDatabase request.
            Operation<Database, RestoreDatabaseMetadata> response =
                databaseAdminClient.RestoreDatabase(restoreRequest);

            Console.WriteLine("Waiting for the operation to finish");

            // Poll until the returned long-running operation is complete.
            var completedResponse = response.PollUntilCompleted();

            return completedResponse.Result.RestoreInfo;
        }
        // [END spanner_restore_database]
    }
}
