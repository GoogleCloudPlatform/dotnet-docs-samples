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

// [START spanner_postgresql_dml_with_parameters]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class InsertUsingDmlWithParametersAsyncPostgresSample
{
    public async Task<int> InsertUsingDmlWithParametersAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateDmlCommand("INSERT INTO Singers (SingerId, FirstName, LastName) " +
                                   "VALUES ($1, $2, $3), " +
                                   "($4, $5, $6)",
                                   new SpannerParameterCollection
                                   {
                                        { "p1", SpannerDbType.Int64, 4 },
                                        { "p2", SpannerDbType.String, "Bruce" },
                                        { "p3", SpannerDbType.String, "Allison" },
                                        { "p4", SpannerDbType.Int64, 5 },
                                        { "p5", SpannerDbType.String, "George" },
                                        { "p6", SpannerDbType.String, "Harrison" }
                                   });

        // The value of count should be 2, as 2 rows are inserted by DML.
        var count = await command.ExecuteNonQueryAsync(); 
        return count;
    }
}
// [END spanner_postgresql_dml_with_parameters]
