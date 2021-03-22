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

using System.Threading.Tasks;
using Xunit;
using Google.Cloud.Spanner.Admin.Database.V1;

/// <summary>
/// Tests creating, backing up and restoring databases using customer managed encryption.
/// These tests are executed in alphabetical order to guarantee that the same database and
/// backup can be used for testing both creating, backing up and restoring, as these
/// operations can take a long time.
/// </summary>
[TestCaseOrderer("Spanner.Samples.Tests.AlphabeticalTestOrderer", "Spanner.Samples.Tests")]
public class DatabaseAndBackupWithEncryptionKeyAsyncTest : IClassFixture<CmekFixture>
{
    private readonly CmekFixture _fixture;

    public DatabaseAndBackupWithEncryptionKeyAsyncTest(CmekFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test01_CreateDatabaseWithEncryptionKeyAsync()
    {
        // Create a database with a custom encryption key.
        var sample = new CreateDatabaseWithEncryptionKeyAsyncSample();
        var database = await sample.CreateDatabaseWithEncryptionKeyAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId, _fixture.KmsKeyName);
        Assert.Equal(_fixture.KmsKeyName, CryptoKeyName.Parse(database.EncryptionConfig.KmsKeyName));
    }

    [Fact]
    public async Task Test02_CreateBackupWithEncryptionKeyAsync()
    {
        // Backup a database with a custom encryption key.
        var sample = new CreateBackupWithEncryptionKeyAsyncSample();
        var backup = await sample.CreateBackupWithEncryptionKeyAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId, _fixture.BackupId, _fixture.KmsKeyName);
        Assert.Equal(_fixture.KmsKeyName.CryptoKeyId, backup.EncryptionInfo.KmsKeyVersionAsCryptoKeyVersionName.CryptoKeyId);
    }

    [Fact]
    public async Task Test03_RestoreDatabaseWithEncryptionKeyAsync()
    {
        var sample = new RestoreDatabaseWithEncryptionAsyncSample();
        var database = await sample.RestoreDatabaseWithEncryptionAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.RestoreDatabaseId, _fixture.BackupId, _fixture.KmsKeyName);
        Assert.Equal(_fixture.KmsKeyName, CryptoKeyName.Parse(database.EncryptionConfig.KmsKeyName));
    }
}
