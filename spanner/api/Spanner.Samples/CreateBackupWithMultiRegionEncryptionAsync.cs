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

// [START spanner_create_backup_with_MR_CMEK]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateBackupWithMultiRegionEncryptionAsyncSample
{
    public async Task<Backup> CreateBackupWithMultiRegionEncryptionAsync(string projectId, string instanceId, string databaseId, string backupId, IEnumerable<CryptoKeyName> kmsKeyNames)
    {
        // Create a DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        // Create the CreateBackupRequest with encryption configuration.
        CreateBackupRequest request = new CreateBackupRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            BackupId = backupId,
            Backup = new Backup
            {
                DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
                ExpireTime = DateTime.UtcNow.AddDays(14).ToTimestamp(),
            },
            EncryptionConfig = new CreateBackupEncryptionConfig
            {
                EncryptionType = CreateBackupEncryptionConfig.Types.EncryptionType.CustomerManagedEncryption,
                KmsKeyNamesAsCryptoKeyNames = { kmsKeyNames },
            },
        };
        // Execute the CreateBackup request.
        var operation = await databaseAdminClient.CreateBackupAsync(request);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating backup: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        var backup = completedResponse.Result;
        Console.WriteLine($"Backup {backup.Name} of size {backup.SizeBytes} bytes was created with encryption keys {string.Join(", ", kmsKeyNames)} at {backup.CreateTime}");
        return backup;
    }
}
// [END spanner_create_backup_with_MR_CMEK]
