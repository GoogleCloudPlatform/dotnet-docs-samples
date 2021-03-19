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

using Google.Cloud.Spanner.Admin.Database.V1;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[TestCaseOrderer("AlphabeticalTestOrderer", "Spanner.Samples.Tests")]
[Collection(nameof(SpannerFixture))]
public class DatabaseAndBackupWithEncryptionKeyAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    private string _testKeyProjectId = Environment.GetEnvironmentVariable("spanner.test.key.project");
    private readonly string _testKeyLocationId = Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1";
    private readonly string _testKeyRingId = Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring";
    private readonly string _testKeyId = Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key";

    private readonly string _databaseId = $"my-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    private readonly string _backupId = $"my-backup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    private readonly string _restoreDatabaseId = $"restored-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    public DatabaseAndBackupWithEncryptionKeyAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        if (_testKeyProjectId == null)
        {
            _testKeyProjectId = _spannerFixture.ProjectId;
        }
    }

    [Fact]
    public async Task Test01_CreateDatabaseWithEncryptionKeyAsync()
    {
        // Create a database with a custom encryption key.
        var sample = new CreateDatabaseWithEncryptionKeyAsyncSample();
        var kmsKeyName = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);
        await sample.CreateDatabaseWithEncryptionKeyAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _databaseId, kmsKeyName);
        var databases = _spannerFixture.GetDatabases();
        Assert.Contains(databases, d => d.DatabaseName.DatabaseId == _databaseId);
        var database = databases.Where(d => d.DatabaseName.DatabaseId == _databaseId).FirstOrDefault();
        Assert.Equal(kmsKeyName, CryptoKeyName.Parse(database.EncryptionConfig.KmsKeyName));
    }

    [Fact]
    public async Task Test02_CreateBackupWithEncryptionKeyAsync()
    {
        // Backup a database with a custom encryption key.
        var sample = new CreateBackupWithEncryptionKeyAsyncSample();
        var kmsKeyName = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);
        var backup = await sample.CreateBackupWithEncryptionKeyAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _databaseId, _backupId, kmsKeyName);
        Assert.Equal(kmsKeyName.CryptoKeyId, backup.EncryptionInfo.KmsKeyVersionAsCryptoKeyVersionName.CryptoKeyId);
    }

    [Fact]
    public async Task Test03_RestoreDatabaseWithEncryptionKeyAsync()
    {
        var sample = new RestoreDatabaseWithEncryptionAsyncSample();
        var kmsKeyName = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);
        var database = await sample.RestoreDatabaseWithEncryptionAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _restoreDatabaseId, _backupId, kmsKeyName);
        Assert.Equal(kmsKeyName, CryptoKeyName.Parse(database.EncryptionConfig.KmsKeyName));
    }
}
