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

// [START spanner_query_data_with_array_of_struct]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryDataWithArrayOfStructAsyncSample
{
    public async Task<List<int>> QueryDataWithArrayOfStructAsync(string projectId, string instanceId, string databaseId)
    {
        // [START spanner_create_user_defined_struct] 
        var nameType = new SpannerStruct
        {
            { "FirstName", SpannerDbType.String, null},
            { "LastName", SpannerDbType.String, null}
        };
        // [END spanner_create_user_defined_struct] 

        // [START spanner_create_array_of_struct_with_data]
        var bandMembers = new List<SpannerStruct>
        {
            new SpannerStruct { { "FirstName", SpannerDbType.String, "Elena" }, { "LastName", SpannerDbType.String, "Campbell" } },
            new SpannerStruct { { "FirstName", SpannerDbType.String, "Gabriel" }, { "LastName", SpannerDbType.String, "Wright" } },
            new SpannerStruct { { "FirstName", SpannerDbType.String, "Benjamin" }, { "LastName", SpannerDbType.String, "Martinez" } },
        };
        // [END spanner_create_array_of_struct_with_data]

        var singerIds = new List<int>();
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        using var cmd = connection.CreateSelectCommand(
            "SELECT SingerId FROM Singers WHERE STRUCT<FirstName STRING, LastName STRING> "
            + "(FirstName, LastName) IN UNNEST(@names)");
        cmd.Parameters.Add("names", SpannerDbType.ArrayOf(nameType.GetSpannerDbType()), bandMembers);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            singerIds.Add(reader.GetFieldValue<int>("SingerId"));
        }
        return singerIds;
    }
}
// [END spanner_query_data_with_array_of_struct]
