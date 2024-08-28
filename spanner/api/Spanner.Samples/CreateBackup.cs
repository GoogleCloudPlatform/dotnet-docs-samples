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

// [START spanner_create_backup]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateBackupSample
{
    public Backup CreateBackup(string projectId, string instanceId, string databaseId, string backupId, DateTime versionTime)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        // Initialize request parameters.
        Backup backup = new Backup
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
            ExpireTime = DateTime.UtcNow.AddDays(14).ToTimestamp(),
            VersionTime = versionTime.ToTimestamp(),
        };
        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);

        // Make the CreateBackup request.
        Operation<Backup, CreateBackupMetadata> response = databaseAdminClient.CreateBackup(instanceName, backup, backupId);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        Operation<Backup, CreateBackupMetadata> completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating backup: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Backup created successfully.");

        // GetBackup to get more information about the created backup.
        BackupName backupName = BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);
        backup = databaseAdminClient.GetBackup(backupName);
        Console.WriteLine($"Backup {backup.Name} of size {backup.SizeBytes} bytes " +
                      $"was created at {backup.CreateTime} from {backup.Database} " +
                      $"and is in state {backup.State} " +
                      $"and has version time {backup.VersionTime}");
        return backup;
    }
}
// [END spanner_create_backup]
