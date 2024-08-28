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
    public const string CreateSingersTableStatement =
    @"CREATE TABLE Singers (
            SingerId INT64 NOT NULL,
            FirstName STRING(1024),
            LastName STRING(1024)
            ) PRIMARY KEY (SingerId)";

    public const string CreateAlbumsTableStatement =
    @"CREATE TABLE Albums (
        SingerId INT64 NOT NULL,
        AlbumId INT64 NOT NULL,
        AlbumTitle STRING(MAX)
        ) PRIMARY KEY (SingerId, AlbumId)";

    public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    // Allow environment variables to override the default instance and database names.
    public string InstanceId { get; } = Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";

    public string DatabaseId { get; private set; }
    public string PostgreSqlDatabaseId { get; private set; }

    public string BackupDatabaseId { get; } = "my-test-database";
    public string BackupId { get; } = "my-test-database-backup";
    public string CreateCustomInstanceConfigId { get; } = $"custom-name-create-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string UpdateCustomInstanceConfigId { get; } = $"custom-name-update-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string DeleteCustomInstanceConfigId { get; } = $"custom-name-delete-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string ToBeCancelledBackupId { get; } = GenerateId("my-backup-");
    public string RestoredDatabaseId { get; private set; }

    public bool SkipCmekBackupSampleTests { get; private set; }
    public const string SkipCmekBackupSamplesMessage = "Spanner CMEK backup sample tests are disabled by default for performance reasons. Set the environment variable RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS=true to enable the test.";

    public string EncryptedDatabaseId { get; private set; }
    public string EncryptedBackupId { get; } = GenerateId("my-enc-backup-");
    public string EncryptedRestoreDatabaseId { get; private set; }

    // These are intentionally kept on the instance to avoid the need to create a new encrypted database and backup for each run.
    public string FixedEncryptedDatabaseId { get; } = "fixed-enc-backup-db";
    public string FixedEncryptedBackupId { get; } = "fixed-enc-backup";

    public string MultiRegionEncryptedDatabaseId { get; private set; }
    public string MultiRegionEncryptedBackupId { get; } = GenerateId("my-mr-enc-backup-");
    public string MultiRegionEncryptedRestoreDatabaseId { get; private set; }

    // These are intentionally kept on the instance to avoid the need to create a new encrypted database and backup for each run.
    public string FixedMultiRegionEncryptedDatabaseId { get; } = "fixed-mr-backup-db";
    public string FixedMultiRegionEncryptedBackupId { get; } = "fixed-mr-backup";

    public CryptoKeyName KmsKeyName { get; } = new CryptoKeyName(
        Environment.GetEnvironmentVariable("spanner.test.key.project") ?? Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"),
        Environment.GetEnvironmentVariable("spanner.test.key.location") ?? "us-central1",
        Environment.GetEnvironmentVariable("spanner.test.key.ring") ?? "spanner-test-keyring",
        Environment.GetEnvironmentVariable("spanner.test.key.name") ?? "spanner-test-key");
    public CryptoKeyName[] KmsKeyNames { get; private set; }

    public string InstanceIdWithProcessingUnits { get; } = GenerateId("my-ins-pu-");
    public string InstanceIdWithMultiRegion { get; } = GenerateId("my-ins-mr-");
    public string InstanceIdWithInstancePartition { get; } = GenerateId("my-ins-prt-");
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
        DatabaseId = Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? GenerateTempDatabaseId();
        PostgreSqlDatabaseId = Environment.GetEnvironmentVariable("TEST_SPANNER_POSTGRESQL_DATABASE") ?? GenerateTempDatabaseId("my-db-pg-");
        RestoredDatabaseId = GenerateTempDatabaseId("my-restore-db-");
        EncryptedDatabaseId = GenerateTempDatabaseId("my-enc-db-");
        EncryptedRestoreDatabaseId = GenerateTempDatabaseId("my-enc-r-db-");
        MultiRegionEncryptedDatabaseId = GenerateTempDatabaseId("my-mr-db-");
        MultiRegionEncryptedRestoreDatabaseId = GenerateTempDatabaseId("my-mr-r-db-");
        KmsKeyNames = new CryptoKeyName[] { KmsKeyName };

        DatabaseAdminClient = await DatabaseAdminClient.CreateAsync();
        InstanceAdminClient = await InstanceAdminClient.CreateAsync();
        AdminSpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}");
        SpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}");
        PgSpannerConnection = new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{PostgreSqlDatabaseId}");

        bool.TryParse(Environment.GetEnvironmentVariable("RUN_SPANNER_CMEK_BACKUP_SAMPLES_TESTS"), out var runCmekBackupSampleTests);
        SkipCmekBackupSampleTests = !runCmekBackupSampleTests;

        await MaybeInitializeTestInstanceAsync();
        await CreateInstanceWithMultiRegionAsync();
        await InitializeInstanceAsync(InstanceIdWithInstancePartition);
        await InitializeDatabaseAsync();
        await InitializeBackupAsync();
        await InitializePostgreSqlDatabaseAsync();

        // Create encryption key for creating an encrypted database and optionally backing up and restoring an encrypted database.
        await InitializeEncryptionKeys();
        if (!SkipCmekBackupSampleTests)
        {
            await InitializeEncryptedBackupAsync();
            await InitializeMultiRegionEncryptedBackupAsync();
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            IList<Task> cleanupTasks = new List<Task>
            {
                DeleteInstanceAsync(InstanceIdWithMultiRegion),
                DeleteInstanceAsync(InstanceIdWithProcessingUnits),
                DeleteInstanceAsync(InstanceIdWithInstancePartition),
                DeleteBackupAsync(ToBeCancelledBackupId),
                DeleteBackupAsync(EncryptedBackupId),
                DeleteBackupAsync(MultiRegionEncryptedRestoreDatabaseId)
            };
            DeleteInstanceConfig(CreateCustomInstanceConfigId);
            DeleteInstanceConfig(UpdateCustomInstanceConfigId);
            DeleteInstanceConfig(DeleteCustomInstanceConfigId);

            foreach (string id in TempDbIds)
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

    private void DeleteInstanceConfig(string instanceConfigId)
    {
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();
        var instanceConfigName = new InstanceConfigName(ProjectId, instanceConfigId);

        try
        {
            instanceAdminClient.DeleteInstanceConfig(new DeleteInstanceConfigRequest
            {
                InstanceConfigName = instanceConfigName
            });
        }
        catch (Exception)
        {
            // Silently ignore errors to prevent tests from failing.
        }
    }

    private async Task<bool> MaybeInitializeTestInstanceAsync()
    {
        InstanceName instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        try
        {
            Instance instance = await InstanceAdminClient.GetInstanceAsync(instanceName);
            if (instance.Edition != Instance.Types.Edition.EnterprisePlus)
            {
                throw new InvalidOperationException($"Test instance with name {instanceName} must be {Instance.Types.Edition.EnterprisePlus}.");
            }
            return true;
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            await InitializeInstanceAsync(InstanceId);
            return false;
        }
    }

    private async Task InitializeInstanceAsync(string instanceId)
    {
        CreateInstanceAsyncSample createInstanceSample = new CreateInstanceAsyncSample();
        await SafeAdminAsync(() => createInstanceSample.CreateInstanceAsync(ProjectId, instanceId, Instance.Types.Edition.EnterprisePlus));
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
            Edition = Instance.Types.Edition.EnterprisePlus,
        };

        await SafeAdminAsync(async () =>
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

    public async Task InitializeTempDatabaseAsync(string databaseId)
    {
        InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
        AddColumnAsyncSample addColumnAsyncSample = new AddColumnAsyncSample();
        await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, databaseId);
        await addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, databaseId);
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
        CreateDatabaseAsyncPostgresSample createDatabaseAsyncSample = new CreateDatabaseAsyncPostgresSample();
        await createDatabaseAsyncSample.CreateDatabaseAsyncPostgres(ProjectId, InstanceId, PostgreSqlDatabaseId);
        await CreateVenueTablesAndInsertDataAsyncPostgres();
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

    private async Task InitializeMultiRegionEncryptedBackupAsync()
    {
        // Sample backup for MR CMEK restore test.
        try
        {
            CreateDatabaseWithMultiRegionEncryptionAsyncSample createDatabaseAsyncSample = new CreateDatabaseWithMultiRegionEncryptionAsyncSample();
            InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
            await createDatabaseAsyncSample.CreateDatabaseWithMultiRegionEncryptionAsync(ProjectId, InstanceId, FixedMultiRegionEncryptedDatabaseId, KmsKeyNames);
            await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, FixedMultiRegionEncryptedDatabaseId);
        }
        catch (Exception e) when (e.ToString().Contains("Database already exists"))
        {
            // We intentionally keep an existing database around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Database {FixedMultiRegionEncryptedDatabaseId} already exists.");
        }

        try
        {
            CreateBackupWithMultiRegionEncryptionAsyncSample createBackupSample = new CreateBackupWithMultiRegionEncryptionAsyncSample();
            await createBackupSample.CreateBackupWithMultiRegionEncryptionAsync(ProjectId, InstanceId, FixedMultiRegionEncryptedDatabaseId, FixedMultiRegionEncryptedBackupId, KmsKeyNames);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
        {
            // We intentionally keep an existing backup around to reduce
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Backup {FixedMultiRegionEncryptedBackupId} already exists.");
        }
    }

    private async Task DeleteInstanceAsync(string instanceId)
    {
        try
        {
            await InstanceAdminClient.DeleteInstanceAsync(InstanceName.FromProjectInstance(ProjectId, instanceId));
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            Console.WriteLine($"Instance {instanceId} was not found for deletion.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while attempting to delete instance {instanceId}");
            Console.WriteLine(ex);
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
            Console.WriteLine($"Backup {backupId} was not found for deletion.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while attempting to delete backup {backupId}");
            Console.WriteLine(ex);
        }
    }

    private async Task DeleteDatabaseAsync(string databaseId)
    {
        try
        {
            using var cmd = AdminSpannerConnection.CreateDdlCommand($@"DROP DATABASE {databaseId}");
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while attempting to delete database {databaseId}");
            Console.WriteLine(ex);
        }
    }

    public static string GenerateId(string prefix, int maxLength = 30) =>
        // Guid.ToString("N") returns a string of 32 digits.
        $"{prefix}{Guid.NewGuid().ToString("N").Substring(prefix.Length + (32 - maxLength))}";

    public string GenerateTempDatabaseId(string prefix = "my-db-")
    {
        string tempDatabaseId = GenerateId(prefix);
        TempDbIds.Add(tempDatabaseId);
        return tempDatabaseId;
    }

    private static readonly string s_retryInfoMetadataKey = RetryInfo.Descriptor.FullName + "-bin";
    public async Task<T> SafeAdminAsync<T>(Func<Task<T>> adminRequestAsync)
    {
        int attempt = 0;
        do
        {
            try
            {
                attempt++;
                return await adminRequestAsync();
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
        var databaseId = GenerateId("temp-db-");
        await RunWithTemporaryDatabaseAsync(instanceId, databaseId, testFunction, extraStatements);
    }

    public async Task RunWithTemporaryDatabaseAsync(string instanceId, string databaseId, Func<string, Task> testFunction, params string[] extraStatements)
    => await RunWithTemporaryDatabaseAsync(instanceId, databaseId, testFunction, DatabaseDialect.GoogleStandardSql, extraStatements);

    private async Task RunWithTemporaryDatabaseAsync(string instanceId, string databaseId, Func<string, Task> testFunction, DatabaseDialect databaseDialect, params string[] extraStatements)
    {
        if(databaseDialect == DatabaseDialect.Postgresql && extraStatements?.Length >=1)
        {
            throw new ArgumentException("Postgres does not accept extra statements for DB creation");
        }
        var databaseCreateRequest = new CreateDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(ProjectId, instanceId),
            CreateStatement = databaseDialect == DatabaseDialect.GoogleStandardSql ? $"CREATE DATABASE `{databaseId}`" : $"CREATE DATABASE \"{databaseId}\"",
            DatabaseDialect = databaseDialect,
        };
        if (databaseDialect == DatabaseDialect.GoogleStandardSql)
        {
            databaseCreateRequest.ExtraStatements.Add(extraStatements);
        }

        var operation = await DatabaseAdminClient.CreateDatabaseAsync(databaseCreateRequest);

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

    public async Task RunWithPostgresqlTemporaryDatabaseAsync(Func<string, Task> testFunction) =>
        await RunWithPostgresqlTemporaryDatabaseAsync(InstanceId, testFunction);

    public async Task RunWithPostgresqlTemporaryDatabaseAsync(string instanceId, Func<string, Task> testFunction)
    {
        var databaseId = GenerateTempDatabaseId();
        await RunWithPostgresqlTemporaryDatabaseAsync(instanceId, databaseId, testFunction);
    }

    public async Task RunWithPostgresqlTemporaryDatabaseAsync(string instanceId, string databaseId, Func<string, Task> testFunction) =>
        await RunWithTemporaryDatabaseAsync(instanceId, databaseId, testFunction, DatabaseDialect.Postgresql);

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

    public async Task CreateSingersAndAlbumsTableAsync(string projectId, string instanceId, string databaseId)
    {
        using var connection = new SpannerConnection($"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}");
        ;
        var createSingersTable =
        @"CREATE TABLE Singers (
                 SingerId INT64 NOT NULL,
                 FirstName STRING(1024),
                 LastName STRING(1024),
                 ComposerInfo BYTES(MAX),
                 FullName STRING(2048) AS (ARRAY_TO_STRING([FirstName, LastName], "" "")) STORED
             ) PRIMARY KEY (SingerId)";

        var createAlbumsTable =
        @"CREATE TABLE Albums (
                 SingerId INT64 NOT NULL,
                 AlbumId INT64 NOT NULL,
                 AlbumTitle STRING(MAX),
                 MarketingBudget INT64
             ) PRIMARY KEY (SingerId, AlbumId),
             INTERLEAVE IN PARENT Singers ON DELETE CASCADE";

        using var createDdlCommand = connection.CreateDdlCommand(createSingersTable, createAlbumsTable);
        await createDdlCommand.ExecuteNonQueryAsync();
    }

    public async Task RefillMarketingBudgetsAsync(int firstAlbumBudget, int secondAlbumBudget, string databaseId = null)
    {
        var spannerConnection = databaseId is null ? SpannerConnection : new SpannerConnection($"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{databaseId}");
        for (int i = 1; i <= 2; ++i)
        {
            var cmd = spannerConnection.CreateUpdateCommand("Albums", new SpannerParameterCollection
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

    private async Task CreateVenueTablesAndInsertDataAsyncPostgres()
    {
        // We create VenueInformation table so that update and query jsonb data sample can run out of order. 

        // Define create table statement for VenueInformation.
        const string createVenueInformationTableStatement =
        @"CREATE TABLE VenueInformation (
            VenueId BIGINT NOT NULL PRIMARY KEY,
            VenueName VARCHAR(1024),
            Details JSONB)";

        await CreateTableAsyncPostgres(createVenueInformationTableStatement);

        // Insert data in VenueInformation table.
        int[] ids = new int[] { 4, 19, 42 };
        await Task.WhenAll(ids.Select(id =>
        {
            using var cmd = PgSpannerConnection.CreateInsertCommand("VenueInformation", new SpannerParameterCollection
            {
                { "VenueId", SpannerDbType.Int64, id },
                { "VenueName", SpannerDbType.String, $"Venue {id}" }
            });
            return cmd.ExecuteNonQueryAsync();
        }));
    }

    public async Task CreateTableAsyncPostgres(string createTableStatement)
    {
        using var cmd = PgSpannerConnection.CreateDdlCommand(createTableStatement);
        await cmd.ExecuteNonQueryAsync();
    }
}
