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

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using CryptoKeyName = Google.Cloud.Spanner.Admin.Database.V1.CryptoKeyName;

public class CmekFixture : IDisposable
{
    // Spanner project and instance id.
    public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public string InstanceId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
    public string DatabaseId { get; } = $"my-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string BackupId { get; } = $"my-backup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string RestoreDatabaseId { get; } = $"my-restore-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    // Encryption key identifiers.
    private static readonly string _testKeyProjectId = Environment.GetEnvironmentVariable("spanner.test.key.project") ?? Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string _testKeyLocationId = Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1";
    private static readonly string _testKeyRingId = Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring";
    private static readonly string _testKeyId = Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key";

    public CryptoKeyName KmsKeyName { get; } = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);

    public CmekFixture()
    {
        MaybeCreateEncryptionKey().WaitWithUnwrappedExceptions();
    }

    private async Task MaybeCreateEncryptionKey()
    {
        var client = await KeyManagementServiceClient.CreateAsync();
        var keyRingName = KeyRingName.FromProjectLocationKeyRing(KmsKeyName.ProjectId, KmsKeyName.LocationId, KmsKeyName.KeyRingId);
        try
        {
            await client.GetKeyRingAsync(keyRingName);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            await client.CreateKeyRingAsync(new CreateKeyRingRequest
            {
                ParentAsLocationName = LocationName.FromProjectLocation(keyRingName.ProjectId, keyRingName.LocationId),
                KeyRingId = KmsKeyName.KeyRingId,
                KeyRing = new KeyRing { KeyRingName = keyRingName },
            });
        }

        var keyName = Google.Cloud.Kms.V1.CryptoKeyName.FromProjectLocationKeyRingCryptoKey(KmsKeyName.ProjectId, KmsKeyName.LocationId, KmsKeyName.KeyRingId, KmsKeyName.CryptoKeyId);
        try
        {
            await client.GetCryptoKeyAsync(keyName);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
        {
            await client.CreateCryptoKeyAsync(new CreateCryptoKeyRequest
            {
                ParentAsKeyRingName = keyRingName,
                CryptoKeyId = keyName.CryptoKeyId,
                CryptoKey = new CryptoKey
                {
                    CryptoKeyName = keyName,
                    Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
                },
            });
        }
    }

    public void Dispose()
    {
        var client = DatabaseAdminClient.Create();
        client.DropDatabase(DatabaseName.FromProjectInstanceDatabase(ProjectId, InstanceId, DatabaseId));
        client.DropDatabase(DatabaseName.FromProjectInstanceDatabase(ProjectId, InstanceId, RestoreDatabaseId));
        client.DeleteBackup(BackupName.FromProjectInstanceBackup(ProjectId, InstanceId, BackupId));
    }
}
