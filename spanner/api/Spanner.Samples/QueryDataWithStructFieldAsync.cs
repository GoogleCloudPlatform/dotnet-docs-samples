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

// [START spanner_field_access_on_struct_parameters]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class QueryDataWithStructFieldAsyncSample
{
    public async Task QueryDataWithStructFieldAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        var structParam = new SpannerStruct
        {
            { "FirstName", SpannerDbType.String, "Elena" },
            { "LastName", SpannerDbType.String, "Campbell" },
        };
        using (var connection = new SpannerConnection(connectionString))
        {
            using (var cmd = connection.CreateSelectCommand("SELECT SingerId FROM Singers WHERE FirstName = @name.FirstName"))
            {
                cmd.Parameters.Add("name", structParam.GetSpannerDbType(), structParam);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetFieldValue<string>("SingerId"));
                    }
                }
            }
        }
    }
}
// [END spanner_field_access_on_struct_parameters]
