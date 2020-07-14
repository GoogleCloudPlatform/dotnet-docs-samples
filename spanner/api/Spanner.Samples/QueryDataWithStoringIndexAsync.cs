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

// [START spanner_read_data_with_storing_index]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class QueryDataWithStoringIndexAsyncSample
{
    public async Task QueryDataWithStoringIndexAsync(string projectId, string instanceId, string databaseId, string startTitle = "Aardvark", string endTitle = "Goo")
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand(
                "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                + "{FORCE_INDEX=AlbumsByAlbumTitle2} "
                + $"WHERE AlbumTitle >= @startTitle "
                + $"AND AlbumTitle < @endTitle",
                new SpannerParameterCollection { { "startTitle", SpannerDbType.String }, { "endTitle", SpannerDbType.String } });
            cmd.Parameters["startTitle"].Value = startTitle;
            cmd.Parameters["endTitle"].Value = endTitle;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine("AlbumId : " + reader.GetFieldValue<string>("AlbumId")
                    + " AlbumTitle : " + reader.GetFieldValue<string>("AlbumTitle")
                    + " MarketingBudget : " + reader.GetFieldValue<string>("MarketingBudget"));
                }
            }
        }
    }
}
// [END spanner_read_data_with_storing_index]
