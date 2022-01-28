// Copyright 2021 Google Inc.
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

// [START spanner_create_backup_with_encryption_key]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

public class CreateBackupWithEncryptionKeyAsyncSample
{
    public async Task<Backup> CreateBackupWithEncryptionKeyAsync(string projectId, string instanceId, string databaseId, string backupId, CryptoKeyName kmsKeyName)
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
                KmsKeyNameAsCryptoKeyName = kmsKeyName,
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
        Console.WriteLine($"Backup {backup.Name} of size {backup.SizeBytes} bytes " +
                      $"was created at {backup.CreateTime} " +
                      $"using encryption key {kmsKeyName}");
        return backup;
    }
}
// [END spanner_create_backup_with_encryption_key]
