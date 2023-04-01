// Copyright 2022 Google Inc.
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

// [START spanner_postgresql_numeric_datatype]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UsePgNumericAsyncPostgresSample
{
    public async Task<List<Venue>> UsePgNumericAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();

        // Create a table that includes a column with data type NUMERIC. As the database has been
        // created with the PostgreSQL dialect, the data type that is used will be the PostgreSQL
        // NUMERIC data type.
        var ddlStatement = @"CREATE TABLE Venues( " +
            "VenueId bigint NOT NULL PRIMARY KEY, " +
            "Name varchar(1024) NOT NULL, " +
            "Revenues numeric)";

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);

        // Create UpdateDatabaseRequest to create the table. 
        var updateDatabaseRequest = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = databaseName,
            Statements = { ddlStatement }
        };

        var updateOperation = await databaseAdminClient.UpdateDatabaseDdlAsync(updateDatabaseRequest);
        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the table to be created.");
        var updateResponse = await updateOperation.PollUntilCompletedAsync();
        if (updateResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating Venues table : {updateResponse.Exception}");
            throw updateResponse.Exception;
        }

        Console.WriteLine("Created Venues table.");

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var batchCommand = connection.CreateBatchDmlCommand();
        // Insert a venue.
        batchCommand.Add("INSERT INTO Venues (VenueId, Name, Revenues) " +
                                    "VALUES ($1, $2, $3)",
                                    new SpannerParameterCollection
                                    {
                                        { "p1", SpannerDbType.Int64, 1 },
                                        { "p2", SpannerDbType.String, "Venue 1" },
                                        { "p3", SpannerDbType.PgNumeric, PgNumeric.Parse("3150.25") }
                                    });
        // Insert a Venue with a NaN (Not a Number) value for the Revenues column.
        batchCommand.Add("INSERT INTO Venues (VenueId, Name, Revenues) " +
                                    "VALUES ($1, $2, $3)",
                                    new SpannerParameterCollection
                                    {
                                        { "p1", SpannerDbType.Int64, 2 },
                                        { "p2", SpannerDbType.String, "Venue 2" },
                                        { "p3", SpannerDbType.PgNumeric, PgNumeric.NaN } // We can also use PgNumeric.Parse("NaN").
                                    });
        // Insert a Venue with a NULL value for the Revenues column.
        batchCommand.Add("INSERT INTO Venues (VenueId, Name, Revenues) " +
                                    "VALUES ($1, $2, $3)",
                                    new SpannerParameterCollection
                                    {
                                        { "p1", SpannerDbType.Int64, 3 },
                                        { "p2", SpannerDbType.String, "Venue 3" },
                                        { "p3", SpannerDbType.PgNumeric, DBNull.Value } // We can use null as well.
                                    });

        var count = await batchCommand.ExecuteNonQueryAsync();

        // Get all Venues and inspect the Revenues values.
        var command = connection.CreateSelectCommand("Select * FROM Venues");

        using var reader = await command.ExecuteReaderAsync();
        List<Venue> result = new List<Venue>();
        while (await reader.ReadAsync())
        {
            result.Add(new Venue
            {
                Id = reader.GetFieldValue<long>("venueid"),
                Name = reader.GetFieldValue<string>("name"),
                // PgNumeric can have DBNull values, so check for DBNull. 
                Revenue = reader.IsDBNull(reader.GetOrdinal("revenues")) ? null : reader.GetFieldValue<PgNumeric?>("revenues")
            });
        }

        return result;
    }

    public struct Venue
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public PgNumeric? Revenue { get; set; }
    }
}
// [END spanner_postgresql_numeric_datatype]
