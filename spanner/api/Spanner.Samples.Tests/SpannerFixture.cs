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
public class SpannerFixture : IDisposable, ICollectionFixture<SpannerFixture>
{
    public string ProjectId { get; set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    // Allow environment variables to override the default instance and database names.
    public string InstanceId { get; set; } = Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
    public string DatabaseId { get; set; } = Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? $"my-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string BackupDatabaseId { get; set; } = "my-test-database";
    public string BackupId { get; set; } = "my-test-database-backup";
    public string ToBeCancelledBackupId { get; set; } = $"my-backup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public string RestoredDatabaseId { get; set; } = $"my-restore-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    public List<string> DatabasesToDelete { get; set; } = new List<string>();

    public SpannerFixture()
    {
        // Don't need to cleanup stale Backups and Databases when instance is new.
        var isExistingInstance = InitializeInstance();
        if (isExistingInstance)
        {
            DeleteStaleBackupsAndDatabases();
        }
        InitializeDatabase();
        InitializeBackup();
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

    private void DeleteStaleBackupsAndDatabases()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName)
            .Where(c => c.DatabaseName.DatabaseId.StartsWith("my-db-") || c.DatabaseName.DatabaseId.StartsWith("my-restore-db-"));
        var databasesToDelete = new List<string>();

        // Delete all the databases created before 4 hrs.
        var timestamp = DateTimeOffset.UtcNow.AddHours(-4).ToUnixTimeMilliseconds();
        foreach (var database in databases)
        {
            var databaseId = database.DatabaseName.DatabaseId.Replace("my-restore-db-", "").Replace("my-db-", "");
            if (long.TryParse(databaseId, out long dbCreationTime) && dbCreationTime <= timestamp)
            {
                databasesToDelete.Add(database.DatabaseName.DatabaseId);
            }
        }

        Console.Out.WriteLineAsync($"{databasesToDelete.Count} old databases found.");

        // Get backups.
        ListBackupsRequest request = new ListBackupsRequest
        {
            ParentAsInstanceName = instanceName,
            Filter = $"database:my-db-"
        };
        var backups = databaseAdminClient.ListBackups(request);

        // Backups that belong to the databases to be deleted.
        var backupsToDelete = backups.Where(c => databasesToDelete.Contains(DatabaseName.Parse(c.Database).DatabaseId));

        Console.Out.WriteLineAsync($"{backupsToDelete.Count()} old backups found.");

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
                DeleteDatabaseAsyncSample deleteDatabaseAsyncSample = new DeleteDatabaseAsyncSample();
                deleteDatabaseAsyncSample.DeleteDatabaseAsync(ProjectId, InstanceId, databaseId).Wait();
            }
            catch (Exception) { }
        }
    }

    private void InitializeDatabase()
    {
        // If the database has not been initialized, retry.
        CreateDatabaseAsyncSample createDatabaseAsyncSample = new CreateDatabaseAsyncSample();
        InsertSampleDataAsyncSample insertSampleDataAsyncSample = new InsertSampleDataAsyncSample();
        InsertStructSampleDataAsyncSample insertStructSampleDataAsyncSample = new InsertStructSampleDataAsyncSample();
        AddColumnAsyncSample addColumnAsyncSample = new AddColumnAsyncSample();
        AddCommitTimestampAsyncSample addCommitTimestampAsyncSample = new AddCommitTimestampAsyncSample();
        AddIndexAsyncSample addIndexAsyncSample = new AddIndexAsyncSample();
        AddStoringIndexAsyncSample addStoringIndexAsyncSample = new AddStoringIndexAsyncSample();
        CreateTableWithDatatypesAsyncSample createTableWithDatatypesAsyncSample = new CreateTableWithDatatypesAsyncSample();
        WriteDatatypesDataAsyncSample writeDatatypesDataAsyncSample = new WriteDatatypesDataAsyncSample();
        createDatabaseAsyncSample.CreateDatabaseAsyncAsync(ProjectId, InstanceId, DatabaseId).Wait();
        insertSampleDataAsyncSample.InsertSampleDataAsync(ProjectId, InstanceId, DatabaseId).Wait();
        insertStructSampleDataAsyncSample.InsertStructSampleDataAsync(ProjectId, InstanceId, DatabaseId).Wait();
        addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, DatabaseId).Wait();
        addCommitTimestampAsyncSample.AddCommitTimestampAsync(ProjectId, InstanceId, DatabaseId).Wait();
        addIndexAsyncSample.AddIndexAsync(ProjectId, InstanceId, DatabaseId).Wait();
        // Create a new table that includes supported datatypes.
        createTableWithDatatypesAsyncSample.CreateTableWithDatatypesAsync(ProjectId, InstanceId, DatabaseId).Wait();
        // Write data to the new table.
        writeDatatypesDataAsyncSample.WriteDatatypesDataAsync(ProjectId, InstanceId, DatabaseId).Wait();
        // Add storing Index on table.
        addStoringIndexAsyncSample.AddStoringIndexAsync(ProjectId, InstanceId, DatabaseId).Wait();
        // Update the value of MarketingBudgets.
        RefillMarketingBudgetsAsync(300000, 300000).Wait();
        DatabasesToDelete.Add(DatabaseId);
    }

    private void InitializeBackup()
    {
        // Sample database for backup and restore tests.
        try
        {
            CreateDatabaseAsyncSample createDatabaseAsyncSample = new CreateDatabaseAsyncSample();
            InsertSampleDataAsyncSample insertSampleDataAsyncSample = new InsertSampleDataAsyncSample();
            createDatabaseAsyncSample.CreateDatabaseAsyncAsync(ProjectId, InstanceId, BackupDatabaseId).Wait();
            insertSampleDataAsyncSample.InsertSampleDataAsync(ProjectId, InstanceId, BackupDatabaseId).Wait();
        }
        catch (Exception e)
        {
            // We intentionally keep an existing database around to reduce the
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            if (e.ToString().Contains("Database already exists"))
            {
                Console.WriteLine($"Database {BackupDatabaseId} already exists.");
            }
            else
            {
                throw;
            }
        }

        try
        {
            CreateBackupSample createBackupSample = new CreateBackupSample();
            createBackupSample.CreateBackup(ProjectId, InstanceId, BackupDatabaseId, BackupId);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
        {
            // We intentionally keep an existing backup around to reduce the
            // the likelihood of test timeouts when creating a backup so
            // it's ok to get an AlreadyExists error.
            Console.WriteLine($"Backup {BackupId} already exists.");
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.FailedPrecondition)
        {
            // It's ok backup has been in progress in another test cycle.
            if (e.Message.Contains("maximum number of pending backups (1) for the database has been reached"))
            {
                Console.WriteLine($"Backup {BackupId} already in progress.");
            }
            else
            {
                throw;
            }
        }
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
        string connectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}";
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();
            var cmd = connection.CreateUpdateCommand("Albums",
            new SpannerParameterCollection
            {
                    {"SingerId", SpannerDbType.Int64},
                    {"AlbumId", SpannerDbType.Int64},
                    {"MarketingBudget", SpannerDbType.Int64},
            });
            for (int i = 1; i <= 2; ++i)
            {
                cmd.Parameters["SingerId"].Value = i;
                cmd.Parameters["AlbumId"].Value = i;
                cmd.Parameters["MarketingBudget"].Value = i == 1 ? firstAlbumBudget : secondAlbumBudget;
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public void Dispose()
    {

        DeleteDatabaseAsyncSample deleteDatabaseAsyncSample = new DeleteDatabaseAsyncSample();
        foreach (var databaseId in DatabasesToDelete)
        {
            try
            {
                deleteDatabaseAsyncSample.DeleteDatabaseAsync(ProjectId, InstanceId, databaseId).Wait();
            }
            catch (Exception) { }
        }
    }
}
