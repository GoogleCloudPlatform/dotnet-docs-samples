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

// [START spanner_write_data_for_struct_queries]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InsertStructSampleDataAsyncSample
{
    public async Task InsertStructSampleDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Singer> singers = new List<Singer> {
            new Singer {SingerId = 6, FirstName = "Elena", LastName = "Campbell"},
            new Singer {SingerId = 7, FirstName = "Gabriel", LastName = "Wright"},
            new Singer {SingerId = 8, FirstName = "Benjamin", LastName = "Martinez"},
            new Singer {SingerId = 9, FirstName = "Hannah", LastName = "Harris"},
        };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Insert rows into the Singers table.
            var cmd = connection.CreateInsertCommand("Singers",
                new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"FirstName", SpannerDbType.String},
                        {"LastName", SpannerDbType.String}
            });
            await Task.WhenAll(singers.Select(singer =>
            {
                cmd.Parameters["SingerId"].Value = singer.SingerId;
                cmd.Parameters["FirstName"].Value = singer.FirstName;
                cmd.Parameters["LastName"].Value = singer.LastName;
                return cmd.ExecuteNonQueryAsync();
            }));
            Console.WriteLine("Inserted struct data.");
        }
    }
}
// [END spanner_write_data_for_struct_queries]
