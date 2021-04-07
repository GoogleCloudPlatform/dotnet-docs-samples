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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CryptoKeyName = Google.Cloud.Spanner.Admin.Database.V1.CryptoKeyName;

[CollectionDefinition(nameof(SpannerFixture))]
public class SpannerFixture : IAsyncLifetime, ICollectionFixture<SpannerFixture>
{
    public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    // Allow environment variables to override the default instance and database names.
    public string InstanceId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
    public string DatabaseId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? $"my-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string BackupDatabaseId { get; } = "my-test-database";
    public string BackupId { get; } = "my-test-database-backup";
    public string ToBeCancelledBackupId { get; } = $"my-backup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string RestoredDatabaseId { get; } = $"my-restore-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    public bool RunCmekBackupSampleTests { get; private set; }
    public const string SkipCmekBackupSamplesMessage = "Spanner CMEK backup sample tests are disabled by default for performance reasons. Set the environment variable RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS=true to enable the test.";
    public string EncryptedDatabaseId { get; } = $"my-enc-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string EncryptedBackupId { get; } = $"my-enc-backup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    // 'restore' is abbreviated to prevent the name from becoming longer than 30 characters.
    public string EncryptedRestoreDatabaseId { get; } = $"my-enc-r-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

    // These are intentionally kept on the instance to avoid the need to create a new encrypted database and backup for each run.
    public string FixedEncryptedDatabaseId { get; } = "fixed-enc-backup-db";
    public string FixedEncryptedBackupId { get; } = "fixed-enc-backup";

    // Encryption key identifiers.
    private static readonly string _testKeyProjectId = Environment.GetEnvironmentVariable("spanner.test.key.project") ?? Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string _testKeyLocationId = Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1";
    private static readonly string _testKeyRingId = Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring";
    private static readonly string _testKeyId = Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key";

    public CryptoKeyName KmsKeyName { get; } = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);

    public string ConnectionString { get; set; }

    public async Task InitializeAsync()
    {
        bool.TryParse(Environment.GetEnvironmentVariable("RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS"), out var runCmekBackupSampleTests);
        RunCmekBackupSampleTests = runCmekBackupSampleTests;

        ConnectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}";
        // Don't need to cleanup stale Backups and Databases when instance is new.
        var isExistingInstance = InitializeInstance();
        if (isExistingInstance)
        {
            await DeleteStaleDatabasesAsync();
            await DeleteStaleBackupsAsync();
        }
        await InitializeDatabaseAsync();
        await InitializeBackupAsync();

        // Create encryption key for creating an encrypted database and optionally backing up and restoring an encrypted database.
        await InitializeEncryptionKeys();
        if (RunCmekBackupSampleTests)
        {
            await InitializeEncryptedBackupAsync();
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private bool InitializeInstance()
    {
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        try
        {
            Instance response = instanceAdminClient.GetInstance(instanceName);
            return true;
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            CreateInstanceSample createInstanceSample = new CreateInstanceSample();
            createInstanceSample.CreateInstance(ProjectId, InstanceId);
            return false;
        }
    }

    /// <summary>
    /// Deletes 10 oldest databases if the number of databases is more than 89.
    /// This is to avoid resource exhausted errors.
    /// </summary>
    private async Task DeleteStaleDatabasesAsync()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName);

        if (databases.Count() < 90)
        {
            return;
        }

        var databasesToDelete = databases
            .OrderBy(db => long.TryParse(
                db.DatabaseName.DatabaseId
                    .Replace("my-db-", "").Replace("my-restore-db-", "")
                    .Replace("my-enc-db-", "").Replace("my-enc-restore-db-", ""),
                out long creationDate) ? creationDate : long.MaxValue)
            .Take(10);

        // Delete the databases.
        foreach (var database in databasesToDelete)
        {
            try
            {
                await DeleteDatabaseAsync(database.DatabaseName.DatabaseId);
            }
            catch (Exception) { }
        }
    }

    /// <summary>
    /// Deletes 10 oldest backups if the number of backups is more than 89.
    /// This is to avoid resource exhausted errors.
    /// </summary>
    private async Task DeleteStaleBackupsAsync()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var backups = databaseAdminClient.ListBackups(instanceName);

        if (backups.Count() < 90)
        {
            return;
        }

        var backupsToDelete = backups
            .OrderBy(db => long.TryParse(
                db.BackupName.BackupId.Replace("my-enc-backup-", ""),
                out long creationDate) ? creationDate : long.MaxValue)
            .Take(10);

        // Delete the backups.
        foreach (var backup in backupsToDelete)
        {
            try
            {
                await databaseAdminClient.DeleteBackupAsync(backup.BackupName);
            }
            catch (Exception) { }
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        // If the database has not been initialized, retry.
        CreateDatabaseAsyncSample createDatabaseAsyncSample = new CreateDatabaseAsyncSample();
        InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
        InsertStructSampleDataAsyncSample insertStructSampleDataAsyncSample = new InsertStructSampleDataAsyncSample();
        AddColumnAsyncSample addColumnAsyncSample = new AddColumnAsyncSample();
        AddCommitTimestampAsyncSample addCommitTimestampAsyncSample = new AddCommitTimestampAsyncSample();
        AddIndexAsyncSample addIndexAsyncSample = new AddIndexAsyncSample();
        AddStoringIndexAsyncSample addStoringIndexAsyncSample = new AddStoringIndexAsyncSample();
        CreateTableWithDatatypesAsyncSample createTableWithDatatypesAsyncSample = new CreateTableWithDatatypesAsyncSample();
        AddNumericColumnAsyncSample addNumericColumnAsyncSample = new AddNumericColumnAsyncSample();
        InsertDatatypesDataAsyncSample insertDatatypesDataAsyncSample = new InsertDatatypesDataAsyncSample();
        CreateTableWithTimestampColumnAsyncSample createTableWithTimestampColumnAsyncSample =
            new CreateTableWithTimestampColumnAsyncSample();
        await createDatabaseAsyncSample.CreateDatabaseAsync(ProjectId, InstanceId, DatabaseId);
        await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, DatabaseId);
        await insertStructSampleDataAsyncSample.InsertStructSampleDataAsync(ProjectId, InstanceId, DatabaseId);
        await addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, DatabaseId);
        await addCommitTimestampAsyncSample.AddCommitTimestampAsync(ProjectId, InstanceId, DatabaseId);
        await addIndexAsyncSample.AddIndexAsync(ProjectId, InstanceId, DatabaseId);
        // Create a new table that includes supported datatypes.
        await createTableWithDatatypesAsyncSample.CreateTableWithDatatypesAsync(ProjectId, InstanceId, DatabaseId);
        // Add Numeric column
        await addNumericColumnAsyncSample.AddNumericColumnAsync(ProjectId, InstanceId, DatabaseId);
        // Write data to the new table.
        await insertDatatypesDataAsyncSample.InsertDatatypesDataAsync(ProjectId, InstanceId, DatabaseId);
        // Add storing Index on table.
        await addStoringIndexAsyncSample.AddStoringIndexAsync(ProjectId, InstanceId, DatabaseId);
        // Update the value of MarketingBudgets.
        await RefillMarketingBudgetsAsync(300000, 300000);
        // Create table with Timestamp column
        await createTableWithTimestampColumnAsyncSample.CreateTableWithTimestampColumnAsync(ProjectId, InstanceId, DatabaseId);
    }

    private async Task InitializeBackupAsync()
    {
        // Sample database for backup and restore tests.
        try
        {
            CreateDatabaseAsyncSample createDatabaseAsyncSample = new CreateDatabaseAsyncSample();
            InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
            await createDatabaseAsyncSample.CreateDatabaseAsync(ProjectId, InstanceId, BackupDatabaseId);
            await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, BackupDatabaseId);
        }
        catch (Exception e) when (e.ToString().Contains("Database already exists"))
        {
            // We intentionally keep an existing database around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Database {BackupDatabaseId} already exists.");
        }

        using var connection = new SpannerConnection(ConnectionString);
        await connection.OpenAsync();
        var currentTimestamp = (DateTime)connection.CreateSelectCommand("SELECT CURRENT_TIMESTAMP").ExecuteScalar();
        try
        {
            CreateBackupSample createBackupSample = new CreateBackupSample();
            createBackupSample.CreateBackup(ProjectId, InstanceId, BackupDatabaseId, BackupId, currentTimestamp);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
        {
            // We intentionally keep an existing backup around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Backup {BackupId} already exists.");
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.FailedPrecondition
        && e.Message.Contains("maximum number of pending backups (1) for the database has been reached"))
        {
            // It's ok backup has been in progress in another test cycle.
            Console.WriteLine($"Backup {BackupId} already in progress.");
        }
    }

    private async Task InitializeEncryptedBackupAsync()
    {
        // Sample backup for encrypted restore test.
        try
        {
            CreateDatabaseWithEncryptionKeyAsyncSample createDatabaseAsyncSample = new CreateDatabaseWithEncryptionKeyAsyncSample();
            InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
            await createDatabaseAsyncSample.CreateDatabaseWithEncryptionKeyAsync(ProjectId, InstanceId, FixedEncryptedDatabaseId, KmsKeyName);
            await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, FixedEncryptedDatabaseId);
        }
        catch (Exception e) when (e.ToString().Contains("Database already exists"))
        {
            // We intentionally keep an existing database around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Database {FixedEncryptedDatabaseId} already exists.");
        }

        try
        {
            CreateBackupWithEncryptionKeyAsyncSample createBackupSample = new CreateBackupWithEncryptionKeyAsyncSample();
            await createBackupSample.CreateBackupWithEncryptionKeyAsync(ProjectId, InstanceId, FixedEncryptedDatabaseId, FixedEncryptedBackupId, KmsKeyName);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
        {
            // We intentionally keep an existing backup around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Backup {FixedEncryptedBackupId} already exists.");
        }
    }

    private async Task InitializeEncryptionKeys()
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
                KeyRing = new KeyRing(),
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
                    Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
                },
            });
        }
    }

    private async Task DeleteDatabaseAsync(string databaseId)
    {
        string adminConnectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}";
        using var connection = new SpannerConnection(adminConnectionString);
        using var cmd = connection.CreateDdlCommand($@"DROP DATABASE {databaseId}");
        await cmd.ExecuteNonQueryAsync();
    }

    public IEnumerable<Database> GetDatabases()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName);
        return databases;
    }

    public async Task RefillMarketingBudgetsAsync(int firstAlbumBudget, int secondAlbumBudget)
    {
        using var connection = new SpannerConnection(ConnectionString);
        await connection.OpenAsync();

        for (int i = 1; i <= 2; ++i)
        {
            var cmd = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, i },
                    { "AlbumId", SpannerDbType.Int64, i },
                    { "MarketingBudget", SpannerDbType.Int64, i == 1 ? firstAlbumBudget : secondAlbumBudget },
                });
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
