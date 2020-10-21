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

// [START spanner_field_access_on_nested_struct_parameters]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryDataWithNestedStructFieldAsyncSample
{
    public async Task<List<int>> QueryDataWithNestedStructFieldAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        SpannerStruct name1 = new SpannerStruct
        {
            { "FirstName", SpannerDbType.String, "Elena" },
            { "LastName", SpannerDbType.String, "Campbell" }
        };
        SpannerStruct name2 = new SpannerStruct
        {
            { "FirstName", SpannerDbType.String, "Hannah" },
            { "LastName", SpannerDbType.String, "Harris" }
        };
        SpannerStruct songInfo = new SpannerStruct
        {
            { "song_name", SpannerDbType.String, "Imagination" },
            { "artistNames", SpannerDbType.ArrayOf(name1.GetSpannerDbType()), new[] { name1, name2 } }
        };

        var singerIds = new List<int>();
        using var connection = new SpannerConnection(connectionString);
        using var cmd = connection.CreateSelectCommand(
            "SELECT SingerId, @song_info.song_name "
            + "FROM Singers WHERE STRUCT<FirstName STRING, LastName STRING>(FirstName, LastName) "
            + "IN UNNEST(@song_info.artistNames)");

        cmd.Parameters.Add("song_info", songInfo.GetSpannerDbType(), songInfo);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var singerId = reader.GetFieldValue<int>("SingerId");
            singerIds.Add(singerId);
            Console.WriteLine($"SingerId: {singerId}");
            Console.WriteLine($"Song Name: {reader.GetFieldValue<string>(1)}");
        }
        return singerIds;
    }
}
// [END spanner_field_access_on_nested_struct_parameters]
