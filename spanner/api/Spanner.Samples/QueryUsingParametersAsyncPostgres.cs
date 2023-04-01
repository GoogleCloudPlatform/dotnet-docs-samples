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

// [START spanner_postgresql_query_parameter]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryUsingParametersAsyncPostgresSample
{
    public async Task<List<Singer>> QueryUsingParametersAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateSelectCommand("SELECT SingerId, FirstName, LastName FROM Singers WHERE LastName LIKE $1",
            new SpannerParameterCollection
            {
                {"p1", SpannerDbType.String, "N%" }
            });

        var list = new List<Singer>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Singer
            {
                // See https://www.postgresql.org/docs/current/sql-syntax-lexical.html#SQL-SYNTAX-IDENTIFIERS
                // to understand why column names are in lower case.
                Id = reader.GetFieldValue<long>("singerid"),
                FirstName = reader.GetFieldValue<string>("firstname"),
                LastName = reader.GetFieldValue<string>("lastname")
            });
        }

        return list;
    }

    public struct Singer
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
// [END spanner_postgresql_query_parameter]
