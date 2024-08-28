// Copyright 2024 Google Inc.
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

// [START spanner_restore_backup_with_MR_CMEK]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RestoreDatabaseWithMultiRegionEncryptionAsyncSample
{
    public async Task<Database> RestoreDatabaseWithMultiRegionEncryptionAsync(string projectId, string instanceId, string databaseId, string backupId, IEnumerable<CryptoKeyName> kmsKeyNames)
    {
        // Create a DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        // Create the RestoreDatabaseRequest with encryption configuration.
        RestoreDatabaseRequest request = new RestoreDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            DatabaseId = databaseId,
            BackupAsBackupName = BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId),
            EncryptionConfig = new RestoreDatabaseEncryptionConfig
            {
                EncryptionType = RestoreDatabaseEncryptionConfig.Types.EncryptionType.CustomerManagedEncryption,
                KmsKeyNamesAsCryptoKeyNames = { kmsKeyNames },
            }
        };
        // Execute the RestoreDatabase request.
        var operation = await databaseAdminClient.RestoreDatabaseAsync(request);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while restoring database: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        var database = completedResponse.Result;
        var restoreInfo = database.RestoreInfo;
        Console.WriteLine($"Database {restoreInfo.BackupInfo.SourceDatabase} restored to {database.Name} from backup {restoreInfo.BackupInfo.Backup} using encryption keys {string.Join(", ", kmsKeyNames)}");
        return database;
    }
}
// [END spanner_restore_backup_with_MR_CMEK]
