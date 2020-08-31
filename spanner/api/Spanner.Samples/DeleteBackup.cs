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

// [START spanner_delete_backup]

using Google.Cloud.Spanner.Admin.Database.V1;
using System;

public class DeleteBackupSample
{
    public void DeleteBackup(string projectId, string instanceId, string backupId)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        // Make the DeleteBackup request.
        BackupName backupName = BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);
        databaseAdminClient.DeleteBackup(backupName);

        Console.WriteLine("Backup deleted successfully.");
    }
}
// [END spanner_delete_backup]
