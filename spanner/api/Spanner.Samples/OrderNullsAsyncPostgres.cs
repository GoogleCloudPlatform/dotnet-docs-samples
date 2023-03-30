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

// [START spanner_postgresql_order_nulls]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderNullsAsyncPostgresSample
{
    public async Task<List<string>> OrderNullsAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var batchCommand = connection.CreateBatchDmlCommand();

        // Insert data in SingersForOrder table.
        batchCommand.Add("INSERT INTO SingersForOrder (SingerId, Name) VALUES (1, 'Alice')");
        batchCommand.Add("INSERT INTO SingersForOrder (SingerId, Name) VALUES (2, 'Bruce')");
        batchCommand.Add("INSERT INTO SingersForOrder (SingerId, Name) VALUES ($1, $2)",
            new SpannerParameterCollection
            {
                {"p1", SpannerDbType.Int64, 3 },
                {"p2", SpannerDbType.String, DBNull.Value }
            });
        await batchCommand.ExecuteNonQueryAsync();

        var orderedNames = new List<string>();

        // This returns the singers in the order Alice, Bruce, null.
        var selectCommand = connection.CreateSelectCommand("SELECT name FROM SingersForOrder ORDER BY name");
        using var reader = await selectCommand.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            orderedNames.Add(reader.IsDBNull(reader.GetOrdinal("name")) ? null
                : reader.GetFieldValue<string>("name"));
        }

        // This returns the singers in the order null, Bruce, Alice.
        selectCommand = connection.CreateSelectCommand("SELECT name FROM SingersForOrder ORDER BY name DESC");
        using var dataReader = await selectCommand.ExecuteReaderAsync();
        while (await dataReader.ReadAsync())
        {
            orderedNames.Add(dataReader.IsDBNull(dataReader.GetOrdinal("name")) ? null
                : dataReader.GetFieldValue<string>("name"));
        }

        // This returns the singers in the order null, Alice, Bruce.
        selectCommand = connection.CreateSelectCommand("SELECT name FROM SingersForOrder ORDER BY name NULLS FIRST");
        using var nullFirstReader = await selectCommand.ExecuteReaderAsync();
        while (await nullFirstReader.ReadAsync())
        {
            orderedNames.Add(nullFirstReader.IsDBNull(nullFirstReader.GetOrdinal("name")) ? null
                : nullFirstReader.GetFieldValue<string>("name"));
        }

        // This returns the singers in the order Bruce, Alice, null. 
        selectCommand = connection.CreateSelectCommand("SELECT name FROM SingersForOrder ORDER BY name NULLS LAST");
        using var nullLastReader = await selectCommand.ExecuteReaderAsync();
        while (await nullLastReader.ReadAsync())
        {
            orderedNames.Add(nullLastReader.IsDBNull(nullLastReader.GetOrdinal("name")) ? null
                : nullLastReader.GetFieldValue<string>("name"));
        }

        return orderedNames;
    }
}
// [END spanner_postgresql_order_nulls]
