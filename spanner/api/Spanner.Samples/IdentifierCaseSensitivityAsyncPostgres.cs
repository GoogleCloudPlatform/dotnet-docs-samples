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

// [START spanner_postgresql_identifier_case_sensitivity]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class IdentifierCaseSensitivityAsyncPostgresSample
{
    public async Task<int> IdentifierCaseSensitivityAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();

        // See https://www.postgresql.org/docs/current/sql-syntax-lexical.html#SQL-SYNTAX-IDENTIFIERS
        // for more information.
        string ddlStatement = "CREATE TABLE Concerts ( " +
               // ConcertId will be folded to `concertid`.
               "ConcertId bigint NOT NULL PRIMARY KEY, " +
               // Location and Time are double-quoted and will therefore retain their
               // mixed case and are case-sensitive. This means that any statement that
               // references any of these columns must use double quotes.
               "\"Location\" varchar(1024) NOT NULL, " +
               "\"Time\"  timestamptz NOT NULL)";

        var updateDatabaseDDLRequest = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
            Statements = { ddlStatement }
        };

        // Update database schema by adding new table.
        var updateOperation = await databaseAdminClient.UpdateDatabaseDdlAsync(updateDatabaseDDLRequest);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the Concerts table to be created.");
        var response = await updateOperation.PollUntilCompletedAsync();
        if (response.IsFaulted)
        {
            Console.WriteLine($"Error while creating table: {response.Exception}");
            throw response.Exception;
        }

        Console.WriteLine($"Created table with case sensitive names in database ${databaseId} using PostgreSQL dialect.");

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // PostgreSQL case sensitivity with mutations.
        // Mutations: Column names in mutations are always case-insensitive, regardless whether the
        // columns were double-quoted or not during creation.
        using var cmd = connection.CreateInsertCommand("Concerts", new SpannerParameterCollection
                {
                        { "ConcertId", SpannerDbType.Int64, 1 },
                        { "Location", SpannerDbType.String, "Venue 1" },
                        { "Time", SpannerDbType.Timestamp, DateTime.UtcNow }
                });
        await cmd.ExecuteNonQueryAsync();

        // PostgreSQL case sensitivity with queries.
        var selectCommand = connection.CreateSelectCommand("SELECT * FROM Concerts");
        using var reader = await selectCommand.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine("ConcertId : " +
            // ConcertId was not double quoted while table creation, so it is automatically folded to lower case.
            // Accessing the column by its name in a result set must therefore use all lower-case letters.
            reader.GetFieldValue<long>("concertid") +
            // Location and Time were double - quoted during creation,
            // and retain their mixed case when returned in a result set.
            " Location : " +
            reader.GetFieldValue<string>("Location") +
            " Time : " +
            reader.GetFieldValue<DateTime>("Time"));
        }

        // PostgreSQL case sensitivity with aliases.
        // Aliases : They are also identifiers, and specifying an alias in double quotes will make the alias retain its case.
        var selectAliasCommand = connection.CreateSelectCommand("SELECT concertid AS \"ConcertId\", \"Location\" AS \"venue\", \"Time\" FROM Concerts");
        using var dataReader = await selectAliasCommand.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            // The aliases are double-quoted and therefore retain their mixed case.
            Console.WriteLine("ConcertId : " +
            dataReader.GetFieldValue<long>("ConcertId") +
            " Location : " +
            dataReader.GetFieldValue<string>("venue") +
            " Time : " +
            dataReader.GetFieldValue<DateTime>("Time"));
        }

        // PostgreSQL case sensitivity with DML statements.
        // DML statements must also follow the PostgreSQL case rules.
        var dmlCommand = connection.CreateDmlCommand("INSERT INTO Concerts (ConcertId, \"Location\", \"Time\") VALUES($1, $2, $3)",
            new SpannerParameterCollection
            {
                { "p1", SpannerDbType.Int64, 2 },
                { "p2", SpannerDbType.String, "Venue 2" },
                { "p3", SpannerDbType.Timestamp, DateTime.UtcNow }
            });

        var count = await dmlCommand.ExecuteNonQueryAsync();
        return count;
    }
}
// [END spanner_postgresql_identifier_case_sensitivity]
