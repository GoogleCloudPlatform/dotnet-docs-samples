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

// [START spanner_cancel_backup_create]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class CancelBackupOperation
    {
        public static object SpannerCancelBackupOperation(
            string projectId, string instanceId, string databaseId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Initialize request parameters.
            Backup backup = new Backup
            {
                DatabaseAsDatabaseName =
                    DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
                ExpireTime = DateTime.UtcNow.AddDays(14).ToTimestamp()
            };
            InstanceName parentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId);

            // Make the CreateBackup request.
            Operation<Backup, CreateBackupMetadata> operation =
                databaseAdminClient.CreateBackup(parentAsInstanceName, backup, backupId);

            // Cancel the operation.
            operation.Cancel();

            // Poll until the long-running operation is complete since the backup wasn't
            // cancelled before it was created.
            Console.WriteLine("Waiting for the operation to finish.");
            Operation<Backup, CreateBackupMetadata> completedOperation =
                operation.PollUntilCompleted();

            if (!completedOperation.IsFaulted)
            {
                Console.WriteLine("Delete backup because it completed before it could be cancelled.");
                BackupName backupAsBackupName =
                    BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);
                databaseAdminClient.DeleteBackup(backupAsBackupName);
            }

            Console.WriteLine($"Create backup operation cancelled: {operation.Name}");
            return 0;
        }
    }
}
// [END spanner_cancel_backup_create]
