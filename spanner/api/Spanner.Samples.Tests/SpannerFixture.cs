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
    public string ConnectionString { get; set; }

    public async Task InitializeAsync()
    {
        ConnectionString = $"Data Source=projects/{ProjectId}/instances/{InstanceId}/databases/{DatabaseId}";
        // Don't need to cleanup stale Backups and Databases when instance is new.
        var isExistingInstance = InitializeInstance();
        if (isExistingInstance)
        {
            await DeleteStaleDatabasesAsync();
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

    /// <summary>
    /// Deletes 5 oldest databases if the number of databases is more than 94.
    /// This is to avoid resource exhausted errors.
    /// </summary>
    private async Task DeleteStaleDatabasesAsync()
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(ProjectId, InstanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName);

        if (databases.Count() < 95)
        {
            return;
        }

        var databasesToDelete = databases
            .OrderBy(db => long.TryParse(
                db.DatabaseName.DatabaseId.Replace("my-db-", "").Replace("my-restore-db-", ""),
                out long creationDate) ? creationDate : long.MaxValue)
            .Take(5);

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
        await InsertStructDataAsync();
        await addColumnAsyncSample.AddColumnAsync(ProjectId, InstanceId, DatabaseId);
        await AddCommitTimestampAsync();
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

    public async Task CreateVenuesTableAndInsertDataAsync()
    {
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection(ConnectionString);

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

    public async Task DeleteVenuesTable()
    {
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection(ConnectionString);

        // Drop the Venues table.
        using var cmd = connection.CreateDdlCommand(@"DROP TABLE Venues");
        await cmd.ExecuteNonQueryAsync();
    }

    private class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
    }

    private async Task InsertStructDataAsync()
    {
        List<Singer> singers = new List<Singer>
        {
            new Singer { SingerId = 6, FirstName = "Elena", LastName = "Campbell" },
            new Singer { SingerId = 7, FirstName = "Gabriel", LastName = "Wright" },
            new Singer { SingerId = 8, FirstName = "Benjamin", LastName = "Martinez" },
            new Singer { SingerId = 9, FirstName = "Hannah", LastName = "Harris" },
            new Singer { SingerId = 10, FirstName = "Anthony", LastName = "Hunter" },
            new Singer { SingerId = 11, FirstName = "Earlean", LastName = "Holland" }
        };

        using var connection = new SpannerConnection(ConnectionString);
        await connection.OpenAsync();

        var rows = await Task.WhenAll(singers.Select(singer =>
        {
            var cmd = connection.CreateInsertCommand("Singers",
              new SpannerParameterCollection
              {
                  { "SingerId", SpannerDbType.Int64, singer.SingerId },
                  { "FirstName", SpannerDbType.String, singer.FirstName },
                  { "LastName", SpannerDbType.String, singer.LastName }
              });

            return cmd.ExecuteNonQueryAsync();
        }));
    }

    private async Task AddCommitTimestampAsync()
    {
        string alterStatement = "ALTER TABLE Albums ADD COLUMN LastUpdateTime TIMESTAMP OPTIONS (allow_commit_timestamp=true)";
        using var connection = new SpannerConnection(ConnectionString);
        var updateCmd = connection.CreateDdlCommand(alterStatement);
        await updateCmd.ExecuteNonQueryAsync();
    }

    private class Singer
    {
        public int SingerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}