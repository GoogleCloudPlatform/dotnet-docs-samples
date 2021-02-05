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

public class CancelBackupOperationSample
{
    public bool CancelBackupOperation(string projectId, string instanceId, string databaseId, string backupId)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        // Initialize backup request parameters.
        Backup backup = new Backup
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
            ExpireTime = DateTime.UtcNow.AddDays(14).ToTimestamp()
        };
        InstanceName parentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId);

        // Make the CreateBackup request.
        Operation<Backup, CreateBackupMetadata> operation = databaseAdminClient.CreateBackup(parentAsInstanceName, backup, backupId);

        // Cancel the operation.
        operation.Cancel();

        // Poll until the long-running operation is completed in case the backup was
        // created before the operation was cancelled.
        Console.WriteLine("Waiting for the operation to finish.");
        Operation<Backup, CreateBackupMetadata> completedOperation = operation.PollUntilCompleted();

        if (!completedOperation.IsFaulted)
        {
            Console.WriteLine("The backup was created before the operation was cancelled. Please delete the backup.");
            BackupName backupAsBackupName = BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);
            databaseAdminClient.DeleteBackup(backupAsBackupName);
        }
        else
        {
            Console.WriteLine($"Create backup operation cancelled: {operation.Name}");
        }

        return completedOperation.IsFaulted;
    }
}
// [END spanner_cancel_backup_create]
