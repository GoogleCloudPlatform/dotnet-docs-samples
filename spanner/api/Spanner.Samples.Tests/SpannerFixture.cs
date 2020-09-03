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

    public async Task InitializeAsync()
    {
        // Don't need to cleanup stale Backups and Databases when instance is new.
        var isExistingInstance = InitializeInstance();
        if (isExistingInstance)
        {
            await DeleteStaleBackupsAndDatabasesAsync();
        }
        await InitializeDatabaseAsync();
        await InitializeBackupAsync();
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

    private async Task DeleteStaleBackupsAndDatabasesAsync()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName)
            .Where(c => c.DatabaseName.DatabaseId.StartsWith("my-db-") || c.DatabaseName.DatabaseId.StartsWith("my-restore-db-"));
        var databasesToDelete = new List<string>();

        // Delete all the databases created before 72 hrs.
        var timestamp = DateTimeOffset.UtcNow.AddHours(-72).ToUnixTimeMilliseconds();
        foreach (var database in databases)
        {
            var databaseId = database.DatabaseName.DatabaseId.Replace("my-restore-db-", "").Replace("my-db-", "");
            if (long.TryParse(databaseId, out long dbCreationTime) && dbCreationTime <= timestamp)
            {
                databasesToDelete.Add(database.DatabaseName.DatabaseId);
            }
        }

        await Console.Out.WriteLineAsync($"{databasesToDelete.Count} old databases found.");

        // Get backups.
        ListBackupsRequest request = new ListBackupsRequest
        {
            ParentAsInstanceName = instanceName,
            Filter = $"database:my-db-"
        };
        var backups = databaseAdminClient.ListBackups(request);

        // Backups that belong to the databases to be deleted.
        var backupsToDelete = backups.Where(c => databasesToDelete.Contains(DatabaseName.Parse(c.Database).DatabaseId));

        await Console.Out.WriteLineAsync($"{backupsToDelete.Count()} old backups found.");

        // Delete the backups.
        foreach (var backup in backupsToDelete)
        {
            try
            {
                DeleteBackupSample deleteBackupSample = new DeleteBackupSample();
                deleteBackupSample.DeleteBackup(ProjectId, InstanceId, backup.BackupName.BackupId);
            }
            catch (Exception) { }
        }

        // Delete the databases.
        foreach (var databaseId in databasesToDelete)
        {
            try
            {
                await DeleteDatabaseAsync(databaseId);
            }
            catch (Exception) { }
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        // If the database has not been initialized, retry.
        CreateDatabaseAsyncSample createDatabaseAsyncSample = new CreateDatabaseAsyncSample();
        InsertDataAsyncSample insertDataAsyncSample = new InsertDataAsyncSample();
        AddColumnAsyncSample addColumnAsyncSample = new AddColumnAsyncSample();
        AddIndexAsyncSample addIndexAsyncSample = new AddIndexAsyncSample();
        AddStoringIndexAsyncSample addStoringIndexAsyncSample = new AddStoringIndexAsyncSample();
        await createDatabaseAsyncSample.CreateDatabaseAsync(ProjectId, InstanceId, DatabaseId);
        await insertDataAsyncSample.InsertDataAsync(ProjectId, InstanceId, DatabaseId);
        await addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, DatabaseId);
        await addIndexAsyncSample.AddIndexAsync(ProjectId, InstanceId, DatabaseId);
        // Add storing Index on table.
        await addStoringIndexAsyncSample.AddStoringIndexAsync(ProjectId, InstanceId, DatabaseId);
        // Update the value of MarketingBudgets.
        await RefillMarketingBudgetsAsync(300000, 300000);
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
            createBackupSample.CreateBackup(ProjectId, InstanceId, BackupDatabaseId, BackupId);
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
        string connectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}/" +
            $"databases/{DatabaseId}";
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        for (int i = 1; i <= 2; ++i)
        {
            var cmd = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64 },
                    { "AlbumId", SpannerDbType.Int64 },
                    { "MarketingBudget", SpannerDbType.Int64 },
                });

            cmd.Parameters["SingerId"].Value = i;
            cmd.Parameters["AlbumId"].Value = i;
            cmd.Parameters["MarketingBudget"].Value = i == 1 ? firstAlbumBudget : secondAlbumBudget;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
