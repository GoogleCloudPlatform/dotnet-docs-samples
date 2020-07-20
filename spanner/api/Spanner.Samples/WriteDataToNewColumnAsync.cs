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

// [START spanner_update_data]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class WriteDataToNewColumnAsyncSample
{
    public async Task<int> WriteDataToNewColumnAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection {
                {"SingerId", SpannerDbType.Int64},
                {"AlbumId", SpannerDbType.Int64},
                {"MarketingBudget", SpannerDbType.Int64}
            });
            var rowCount = 0;
            var cmdLookup = connection.CreateSelectCommand("SELECT * FROM Albums");
            using (var reader = await cmdLookup.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetFieldValue<int>("SingerId") == 1 && reader.GetFieldValue<int>("AlbumId") == 1)
                    {
                        cmd.Parameters["SingerId"].Value = reader.GetFieldValue<int>("SingerId");
                        cmd.Parameters["AlbumId"].Value = reader.GetFieldValue<int>("AlbumId");
                        cmd.Parameters["MarketingBudget"].Value = 100000;
                        rowCount += await cmd.ExecuteNonQueryAsync();
                    }

                    if (reader.GetInt64(0) == 2 && reader.GetInt64(1) == 2)
                    {
                        cmd.Parameters["SingerId"].Value = reader.GetFieldValue<int>("SingerId");
                        cmd.Parameters["AlbumId"].Value = reader.GetFieldValue<int>("AlbumId");
                        cmd.Parameters["MarketingBudget"].Value = 500000;
                        rowCount += await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            Console.WriteLine("Updated data.");
            return rowCount;
        }
    }
}
// [END spanner_update_data]
