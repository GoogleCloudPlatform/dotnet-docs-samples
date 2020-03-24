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
    public class CreateBackup
    {
        // [START spanner_create_backup]
        public static Backup SpannerCreateBackup(Backup backup, string backupId, string parentInstanceId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Initialize Create Backup Request instance.
            var backupRequest = new CreateBackupRequest
            {
                Backup = backup,
                BackupId = backupId,
                Parent = parentInstanceId
            };

            // Make the CreateBackup request.
            Operation<Backup, CreateBackupMetadata> response =
                databaseAdminClient.CreateBackup(backupRequest);

            Console.WriteLine("Waiting for the operation to finish");

            // Poll until the returned long-running operation is complete.
            Operation<Backup, CreateBackupMetadata> completedResponse =
                response.PollUntilCompleted();

            return completedResponse.Result;
        }
        // [END spanner_create_backup]
    }
}
