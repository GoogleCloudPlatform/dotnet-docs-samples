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
using Google.Rpc;
using GoogleCloudSamples;
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

    public string InstanceIdWithProcessingUnits { get; } = $"my-instance-processing-units-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string InstanceIdWithMultiRegion { get; } = $"my-instance-multi-region-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string InstanceConfigId { get; } = "nam6";

    private DatabaseAdminClient DatabaseAdminClient { get; set; }

    public RetryRobot Retryable { get; } = new RetryRobot
    {
        ShouldRetry = ex => ex.IsTransientSpannerFault()
    };

    // Encryption key identifiers.
    private static readonly string _testKeyProjectId = Environment.GetEnvironmentVariable("spanner.test.key.project") ?? Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string _testKeyLocationId = Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1";
    private static readonly string _testKeyRingId = Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring";
    private static readonly string _testKeyId = Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key";

    public CryptoKeyName KmsKeyName { get; } = new CryptoKeyName(_testKeyProjectId, _testKeyLocationId, _testKeyRingId, _testKeyId);

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        DatabaseAdminClient = await DatabaseAdminClient.CreateAsync();

        bool.TryParse(Environment.GetEnvironmentVariable("RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS"), out var runCmekBackupSampleTests);
        RunCmekBackupSampleTests = runCmekBackupSampleTests;

        ConnectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}";
        // Don't need to cleanup stale Backups and Databases when instance is new.
        var isExistingInstance = await InitializeInstanceAsync();
        if (isExistingInstance)
        {
            await DeleteStaleBackupsAsync();
            await DeleteStaleDatabasesAsync();
        }
        await CreateInstanceWithMultiRegionAsync();
        await DeleteStaleInstancesAsync();
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
        DeleteInstance(InstanceIdWithProcessingUnits);
        DeleteInstance(InstanceIdWithMultiRegion);

        return Task.CompletedTask;
    }

    public async Task<T> SafeCreateInstanceAsync<T>(Func<Task<T>> createInstanceAsync)
    {
        int attempt = 0;
        do
        {
            try
            {
                attempt++;
                return await createInstanceAsync();
            }
            catch(RpcException ex) when (attempt <= 10)
            {
                if (StatusCode.Unavailable == ex.StatusCode)
                {
                    await RecommendedDelayAsync(ex);
                }
                else if (StatusCode.ResourceExhausted == ex.StatusCode && ex.Status.Detail.Contains("requests per minute"))
                {
                    await RecommendedDelayAsync(ex, 60);
                }
                else
                {
                    throw;
                }
            }
        }
        while (true);
    }

    private static readonly string s_retryInfoMetadataKey = RetryInfo.Descriptor.FullName + "-bin";
    public static async Task RecommendedDelayAsync(RpcException exception, int fallbackSeconds = 5)
    {
        TimeSpan delay = TimeSpan.FromSeconds(fallbackSeconds);
        var retryInfoEntry = exception.Trailers.FirstOrDefault(
            entry => s_retryInfoMetadataKey.Equals(entry.Key, StringComparison.InvariantCultureIgnoreCase));
        if (retryInfoEntry != null)
        {
            var retryInfo = RetryInfo.Parser.ParseFrom(retryInfoEntry.ValueBytes);
            var recommended = retryInfo.RetryDelay.ToTimeSpan();
            if (recommended != TimeSpan.Zero)
            {
                delay = recommended;
            }
        }
        await Task.Delay(delay);
    }

    private async Task<bool> InitializeInstanceAsync()
    {
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        try
        {
            Instance response = await instanceAdminClient.GetInstanceAsync(instanceName);
            return true;
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            CreateInstanceSample createInstanceSample = new CreateInstanceSample();
            await SafeCreateInstanceAsync(() => Task.FromResult(createInstanceSample.CreateInstance(ProjectId, InstanceId)));
            return false;
        }
    }

    private async Task CreateInstanceWithMultiRegionAsync()
    {
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();

        var projectName = ProjectName.FromProject(ProjectId);
        Instance instance = new Instance
        {
            DisplayName = "Multi-region samples test",
            ConfigAsInstanceConfigName = InstanceConfigName.FromProjectInstanceConfig(ProjectId, InstanceConfigId),
            InstanceName = InstanceName.FromProjectInstance(ProjectId, InstanceIdWithMultiRegion),
            NodeCount = 1,
        };

        await SafeCreateInstanceAsync(async () =>
        {
            var response = await instanceAdminClient.CreateInstanceAsync(projectName, InstanceIdWithMultiRegion, instance);
            // Poll until the returned long-running operation is complete
            response = await response.PollUntilCompletedAsync();
            return response.Result;
        });
    }

    private void DeleteInstance(string instanceId)
    {
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, instanceId);
        try
        {
            instanceAdminClient.DeleteInstance(instanceName);
        }
        catch (Exception)
        {
            // Silently ignore errors to prevent tests from failing.
        }
    }

    private async Task DeleteStaleDatabasesAsync()
    {
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = DatabaseAdminClient.ListDatabases(instanceName, pageSize: 200).ToList();

        if (databases.Count < 50)
        {
            return;
        }

        var deleteCount = Math.Max(30, databases.Count - 50);

        var databasesToDelete = databases
            .OrderBy(db => long.TryParse(
                db.DatabaseName.DatabaseId
                    .Replace("my-db-", "").Replace("my-restore-db-", "")
                    .Replace("my-enc-db-", "").Replace("my-enc-restore-db-", ""),
                out long creationDate) ? creationDate : long.MaxValue)
            .Take(deleteCount);

        // Delete the databases.
        foreach (var database in databasesToDelete)
        {
            try
            {
                await DeleteDatabaseAsync(database.DatabaseName.DatabaseId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to delete stale test database {database.DatabaseName.DatabaseId}: {e.Message}");
            }
        }
    }

    private async Task DeleteStaleBackupsAsync()
    {
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var backups = DatabaseAdminClient.ListBackups(instanceName, pageSize: 200).ToList();

        if (backups.Count < 50)
        {
            return;
        }

        var deleteCount = Math.Max(30, backups.Count - 50);

        var backupsToDelete = backups
            .OrderBy(db => long.TryParse(
                db.BackupName.BackupId.Replace("my-enc-backup-", ""),
                out long creationDate) ? creationDate : long.MaxValue)
            .Take(deleteCount);

        // Delete the backups.
        foreach (var backup in backupsToDelete)
        {
            try
            {
                await DatabaseAdminClient.DeleteBackupAsync(backup.BackupName);
            }
            catch (Exception) { }
        }
    }

    /// <summary>
    /// Deletes 10 oldest instances if the number of instances is more than 14.
    /// This is to clean up the stale instances in case of instance cleanup code may not get triggered.
    /// </summary>
    private async Task DeleteStaleInstancesAsync()
    {
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();
        var listInstancesRequest = new ListInstancesRequest
        {
            Filter = "name:my-instance-processing-units- OR name:my-instance-multi-region-",
            ParentAsProjectName = ProjectName.FromProject(ProjectId)
        };
        var instances = instanceAdminClient.ListInstances(listInstancesRequest);

        if (instances.Count() < 15)
        {
            return;
        }

        long fiveHoursAgo = DateTimeOffset.UtcNow.AddHours(-5).ToUnixTimeMilliseconds();

        var instancesToDelete = instances
            .Select(db => (db, CreationUnixTimeMilliseconds(db)))
            .Where(pair => pair.Item2 < fiveHoursAgo)
            .Select(pair => pair.db);

        // Delete the instances.
        foreach (var instance in instancesToDelete)
        {
            try
            {
                await instanceAdminClient.DeleteInstanceAsync(instance.InstanceName);
            }
            catch (Exception) { }
        }

        static long CreationUnixTimeMilliseconds(Instance db) =>
            long.TryParse(
                db.InstanceName.InstanceId
                    .Replace("my-instance-processing-units-", "")
                    .Replace("my-instance-multi-region-", ""),
                out long creationDate) ? creationDate : long.MaxValue;
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
        CreateTableWithDataTypesAsyncSample createTableWithDataTypesAsyncSample = new CreateTableWithDataTypesAsyncSample();
        InsertDataTypesDataAsyncSample insertDataTypesDataAsyncSample = new InsertDataTypesDataAsyncSample();
        CreateTableWithTimestampColumnAsyncSample createTableWithTimestampColumnAsyncSample =
            new CreateTableWithTimestampColumnAsyncSample();
        await createDatabaseAsyncSample.CreateDatabaseAsync(ProjectId, InstanceId, DatabaseId);
        await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, DatabaseId);
        await insertStructSampleDataAsyncSample.InsertStructSampleDataAsync(ProjectId, InstanceId, DatabaseId);
        await addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, DatabaseId);
        await addCommitTimestampAsyncSample.AddCommitTimestampAsync(ProjectId, InstanceId, DatabaseId);
        await addIndexAsyncSample.AddIndexAsync(ProjectId, InstanceId, DatabaseId);
        // Create a new table that includes supported datatypes.
        await createTableWithDataTypesAsyncSample.CreateTableWithDataTypesAsync(ProjectId, InstanceId, DatabaseId);
        // Write data to the new table.
        await insertDataTypesDataAsyncSample.InsertDataTypesDataAsync(ProjectId, InstanceId, DatabaseId);
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

    public async Task CreateVenuesTableAndInsertDataAsync(string databaseId)
    {
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{databaseId}");

        // Define create table statement for Venues.
        string createTableStatement =
        @"CREATE TABLE Venues (
                 VenueId INT64 NOT NULL,
                 VenueName STRING(1024),
             ) PRIMARY KEY (VenueId)";

        using var cmd = connection.CreateDdlCommand(createTableStatement);
        await cmd.ExecuteNonQueryAsync();

        List<Venue> venues = new List<Venue>
        {
            new Venue { VenueId = 4, VenueName = "Venue 4" },
            new Venue { VenueId = 19, VenueName = "Venue 19" },
            new Venue { VenueId = 42, VenueName = "Venue 42" },
        };

        await Task.WhenAll(venues.Select(venue =>
        {
            // Insert rows into the Venues table.

            using var cmd = connection.CreateInsertCommand("Venues", new SpannerParameterCollection
            {
                { "VenueId", SpannerDbType.Int64, venue.VenueId },
                { "VenueName", SpannerDbType.String, venue.VenueName }
            });
            return cmd.ExecuteNonQueryAsync();
        }));
    }

    private class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
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
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = DatabaseAdminClient.ListDatabases(instanceName);
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

    public async Task RunWithTemporaryDatabaseAsync(Func<string, Task> testFunction, params string[] extraStatements)
        => await RunWithTemporaryDatabaseAsync(InstanceId, testFunction, extraStatements);

    public async Task RunWithTemporaryDatabaseAsync(string instanceId, Func<string, Task> testFunction,
        params string[] extraStatements)
    {
        // For temporary DBs we don't need a time based ID, as we delete them inmediately.
        var databaseId = $"temp-db-{Guid.NewGuid().ToString("N").Substring(0, 20)}";
        await RunWithTemporaryDatabaseAsync(instanceId, databaseId, testFunction, extraStatements);
    }
    
    public async Task RunWithTemporaryDatabaseAsync(string instanceId, string databaseId, Func<string, Task> testFunction, params string[] extraStatements)
    {
        var operation = await DatabaseAdminClient.CreateDatabaseAsync(new CreateDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(ProjectId, instanceId),
            CreateStatement = $"CREATE DATABASE `{databaseId}`",
            ExtraStatements = { extraStatements },
        });
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            throw completedResponse.Exception;
        }

        try
        {
            await testFunction(databaseId);
        }
        finally
        {
            // Cleanup the test database.
            await DatabaseAdminClient.DropDatabaseAsync(DatabaseName.FormatProjectInstanceDatabase(ProjectId, instanceId, databaseId));
        }
    }
}
