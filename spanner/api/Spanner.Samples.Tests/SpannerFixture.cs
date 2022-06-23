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

    public string DatabaseId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? $"my-db-{Guid.NewGuid().ToString("N").Substring(8)}";
    public string PostgreSqlDatabaseId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_POSTGRESQL_DATABASE") ?? $"my-db-pg-{Guid.NewGuid().ToString("N").Substring(11)}";

    public string BackupDatabaseId { get; } = "my-test-database";
    public string BackupId { get; } = "my-test-database-backup";
    public string ToBeCancelledBackupId { get; } = $"my-backup-{Guid.NewGuid().ToString("N").Substring(12)}";
    public string RestoredDatabaseId { get; } = $"my-restore-db-{Guid.NewGuid().ToString("N").Substring(16)}";

    public bool RunCmekBackupSampleTests { get; private set; }
    public const string SkipCmekBackupSamplesMessage = "Spanner CMEK backup sample tests are disabled by default for performance reasons. Set the environment variable RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS=true to enable the test.";

    public string EncryptedDatabaseId { get; } = $"my-enc-db-{Guid.NewGuid().ToString("N").Substring(12)}";
    public string EncryptedBackupId { get; } = $"my-enc-backup-{Guid.NewGuid().ToString("N").Substring(16)}";
    // 'restore' is abbreviated to prevent the name from becoming longer than 30 characters.
    public string EncryptedRestoreDatabaseId { get; } = $"my-enc-r-db-{Guid.NewGuid().ToString("N").Substring(14)}";

    // These are intentionally kept on the instance to avoid the need to create a new encrypted database and backup for each run.
    public string FixedEncryptedDatabaseId { get; } = "fixed-enc-backup-db";
    public string FixedEncryptedBackupId { get; } = "fixed-enc-backup";

    public CryptoKeyName KmsKeyName { get; } = new CryptoKeyName(
        Environment.GetEnvironmentVariable("spanner.test.key.project") ?? Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"),
        Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1",
        Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring",
        Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key");

    public string InstanceIdWithProcessingUnits { get; } = $"my-ins-pu-{Guid.NewGuid().ToString("N").Substring(12)}";
    public string InstanceIdWithMultiRegion { get; } = $"my-ins-mr-{Guid.NewGuid().ToString("N").Substring(12)}";
    public string InstanceConfigId { get; } = "nam6";

    private IList<string> TempDbIds { get; } = new List<string>();

    public DatabaseAdminClient DatabaseAdminClient { get; private set; }
    public InstanceAdminClient InstanceAdminClient { get; private set; }
    public SpannerConnection AdminSpannerConnection { get; private set; }
    public SpannerConnection SpannerConnection { get; private set; }
    public SpannerConnection PgSpannerConnection { get; private set; }

    public RetryRobot Retryable { get; } = new RetryRobot
    {
        ShouldRetry = ex => ex.IsTransientSpannerFault()
    };

    public async Task InitializeAsync()
    {
        DatabaseAdminClient = await DatabaseAdminClient.CreateAsync();
        InstanceAdminClient = await InstanceAdminClient.CreateAsync();
        AdminSpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}");
        SpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}");
        PgSpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{PostgreSqlDatabaseId}");

        bool.TryParse(Environment.GetEnvironmentVariable("RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS"), out var runCmekBackupSampleTests);
        RunCmekBackupSampleTests = runCmekBackupSampleTests;

        await InitializeInstanceAsync();
        await CreateInstanceWithMultiRegionAsync();
        await InitializeDatabaseAsync();
        await InitializeBackupAsync();
        await InitializePostgreSqlDatabaseAsync();

        // Create encryption key for creating an encrypted database and optionally backing up and restoring an encrypted database.
        await InitializeEncryptionKeys();
        if (RunCmekBackupSampleTests)
        {
            await InitializeEncryptedBackupAsync();
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            IList<Task> cleanupTasks = new List<Task>();

            cleanupTasks.Add(DeleteInstanceAsync(InstanceIdWithMultiRegion));
            cleanupTasks.Add(DeleteInstanceAsync(InstanceIdWithProcessingUnits));

            cleanupTasks.Add(DeleteBackupAsync(ToBeCancelledBackupId));
            cleanupTasks.Add(DeleteBackupAsync(EncryptedBackupId));

            cleanupTasks.Add(DeleteDatabaseAsync(DatabaseId));
            cleanupTasks.Add(DeleteDatabaseAsync(PostgreSqlDatabaseId));
            cleanupTasks.Add(DeleteDatabaseAsync(RestoredDatabaseId));
            cleanupTasks.Add(DeleteDatabaseAsync(EncryptedDatabaseId));
            cleanupTasks.Add(DeleteDatabaseAsync(EncryptedRestoreDatabaseId));

            foreach(string id in TempDbIds)
            {
                cleanupTasks.Add(DeleteDatabaseAsync(id));
            }

            await Task.WhenAll(cleanupTasks);
        }
        finally
        {
            AdminSpannerConnection?.Dispose();
            SpannerConnection?.Dispose();
            PgSpannerConnection?.Dispose();
        }
    }

    private async Task<bool> InitializeInstanceAsync()
    {
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        try
        {
            Instance response = await InstanceAdminClient.GetInstanceAsync(instanceName);
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
            var response = await InstanceAdminClient.CreateInstanceAsync(projectName, InstanceIdWithMultiRegion, instance);
            // Poll until the returned long-running operation is complete
            response = await response.PollUntilCompletedAsync();
            return response.Result;
        });
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

        try
        {
            CreateBackupSample createBackupSample = new CreateBackupSample();
            createBackupSample.CreateBackup(ProjectId, InstanceId, BackupDatabaseId, BackupId, DateTime.UtcNow.AddMilliseconds(-500));
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

    private async Task InitializePostgreSqlDatabaseAsync()
    {
        CreateDatabaseAsyncPostgreSample createDatabaseAsyncSample = new CreateDatabaseAsyncPostgreSample();
        await createDatabaseAsyncSample.CreateDatabaseAsyncPostgre(ProjectId, InstanceId, PostgreSqlDatabaseId);
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

    private async Task DeleteInstanceAsync(string instanceId)
    {
        try
        {
            await InstanceAdminClient.DeleteInstanceAsync(InstanceName.FromProjectInstance(ProjectId, instanceId));
        }
        catch(RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            // The instance was not created so tests requiring it should have already failed.
            // Let's not fail here.
        }
    }

    private async Task DeleteBackupAsync(string backupId)
    {
        try
        {
            await DatabaseAdminClient.DeleteBackupAsync(BackupName.FromProjectInstanceBackup(ProjectId, InstanceId, backupId));
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            // The backup was not created so tests requiring it should have already failed.
            // Let's not fail here.
        }
    }

    private async Task DeleteDatabaseAsync(string databaseId)
    {
        using var cmd = AdminSpannerConnection.CreateDdlCommand($@"DROP DATABASE {databaseId}");
        await cmd.ExecuteNonQueryAsync();
    }

    public string GenerateTempDatabaseId()
    {
        string tempDatabaseId = $"my-db-{Guid.NewGuid().ToString("N").Substring(8)}";
        TempDbIds.Add(tempDatabaseId);
        return tempDatabaseId;
    }

    private static readonly string s_retryInfoMetadataKey = RetryInfo.Descriptor.FullName + "-bin";
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
            catch (RpcException ex) when (attempt <= 10)
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

        static async Task RecommendedDelayAsync(RpcException exception, int fallbackSeconds = 5)
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

        int[] ids = new int[] { 4, 19, 42 };
        await Task.WhenAll(ids.Select(id =>
        {
            // Insert rows into the Venues table.

            using var cmd = connection.CreateInsertCommand("Venues", new SpannerParameterCollection
            {
                { "VenueId", SpannerDbType.Int64, id },
                { "VenueName", SpannerDbType.String, $"Venue {id}" }
            });
            return cmd.ExecuteNonQueryAsync();
        }));
    }

    public async Task RefillMarketingBudgetsAsync(int firstAlbumBudget, int secondAlbumBudget)
    {
        for (int i = 1; i <= 2; ++i)
        {
            var cmd = SpannerConnection.CreateUpdateCommand("Albums", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, i },
                    { "AlbumId", SpannerDbType.Int64, i },
                    { "MarketingBudget", SpannerDbType.Int64, i == 1 ? firstAlbumBudget : secondAlbumBudget },
                });
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public IEnumerable<Database> GetDatabases()
    {
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = DatabaseAdminClient.ListDatabases(instanceName);
        return databases;
    }

    
}
