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

// [START spanner_restore_backup]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using System;

public class RestoreDatabaseSample
{
    public RestoreInfo RestoreDatabase(string projectId, string instanceId, string databaseId, string backupId)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        InstanceName parentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        BackupName backupAsBackupName = BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);

        // Make the RestoreDatabase request.
        Operation<Database, RestoreDatabaseMetadata> response = databaseAdminClient.RestoreDatabase(parentAsInstanceName, databaseId, backupAsBackupName);

        Console.WriteLine("Waiting for the operation to finish");

        // Poll until the returned long-running operation is complete.
        var completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Database Restore Failed: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        RestoreInfo restoreInfo = completedResponse.Result.RestoreInfo;
        Console.WriteLine(
            $"Database {restoreInfo.BackupInfo.SourceDatabase} was restored " +
            $"to {databaseId} from backup {restoreInfo.BackupInfo.Backup} " +
            $"with version time {restoreInfo.BackupInfo.VersionTime}");

        return restoreInfo;
    }
}
// [END spanner_restore_backup]
