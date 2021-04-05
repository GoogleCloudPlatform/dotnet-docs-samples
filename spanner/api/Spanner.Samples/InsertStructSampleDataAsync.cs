// Copyright 2021 Google Inc.
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

// [START spanner_write_data_for_struct_queries]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InsertStructSampleDataAsyncSample
{
    public class Singer
    {
        public int SingerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public async Task<int> InsertStructSampleDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Singer> singers = new List<Singer> {
            new Singer { SingerId = 6, FirstName = "Elena", LastName = "Campbell" },
            new Singer { SingerId = 7, FirstName = "Gabriel", LastName = "Wright" },
            new Singer { SingerId = 8, FirstName = "Benjamin", LastName = "Martinez" },
            new Singer { SingerId = 9, FirstName = "Hannah", LastName = "Harris" }
        };

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var rows = await Task.WhenAll(singers.Select(singer =>
        {
            var cmd = connection.CreateInsertCommand("Singers",
                new SpannerParameterCollection {
                    { "SingerId", SpannerDbType.Int64, singer.SingerId },
                    { "FirstName", SpannerDbType.String, singer.FirstName },
                    { "LastName", SpannerDbType.String, singer.LastName }
                });
            return cmd.ExecuteNonQueryAsync();
        }));
        return rows.Sum();
    }
}
// [END spanner_write_data_for_struct_queries]
