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

// [START spanner_copy_backup_with_MR_CMEK]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CopyBackupWithMultiRegionEncryptionAsyncSample
{
    public async Task<Backup> CopyBackupWithMultiRegionEncryptionAsync(
        string sourceProjectId, string sourceInstanceId, string sourceBackupId,
        string targetProjectId, string targetInstanceId, string targetBackupId,
        DateTimeOffset expireTime, IEnumerable<CryptoKeyName> kmsKeyNames)
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        var request = new CopyBackupRequest
        {
            SourceBackupAsBackupName = new BackupName(sourceProjectId, sourceInstanceId, sourceBackupId),
            ParentAsInstanceName = new InstanceName(targetProjectId, targetInstanceId),
            BackupId = targetBackupId,
            ExpireTime = Timestamp.FromDateTimeOffset(expireTime),
            EncryptionConfig = new CopyBackupEncryptionConfig
            {
                EncryptionType = CopyBackupEncryptionConfig.Types.EncryptionType.CustomerManagedEncryption,
                KmsKeyNamesAsCryptoKeyNames = { kmsKeyNames },
            }
        };

        // Execute the CopyBackup request.
        var operation = await databaseAdminClient.CopyBackupAsync(request);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        var completedResponse = await operation.PollUntilCompletedAsync();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while copying backup: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Backup backup = completedResponse.Result;

        Console.WriteLine($"Backup copied successfully.");
        Console.WriteLine($"Backup with Id {sourceBackupId} has been copied from {sourceProjectId}/{sourceInstanceId} to {targetProjectId}/{targetInstanceId} Backup {targetBackupId}");
        Console.WriteLine($"Backup {backup.Name} of size {backup.SizeBytes} bytes was created with encryption keys {string.Join(", ", kmsKeyNames)} at {backup.CreateTime} from {backup.Database} and is in state {backup.State} and has version time {backup.VersionTime}");

        return backup;
    }
}

// [END spanner_copy_backup_with_MR_CMEK]
