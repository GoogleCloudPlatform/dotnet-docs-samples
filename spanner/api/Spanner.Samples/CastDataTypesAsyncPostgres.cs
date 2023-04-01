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

// [START spanner_postgresql_cast_data_type]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class CastDataTypesAsyncPostgresSample
{
    public async Task<CastDataType> CastDataTypesAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // The `::` cast operator can be used to cast from one data type to another.
        var commandText = "SELECT 1::varchar as str, '2'::int as int, 3::decimal as dec, '4'::bytea as bytes, "
                          + "5::float as float, 'true'::bool as bool, '2021-11-03T09:35:01UTC'::timestamptz as timestamp";
        var command = connection.CreateSelectCommand(commandText);

        using var reader = await command.ExecuteReaderAsync();
        var result = new CastDataType();
        while (await reader.ReadAsync())
        {
            result.String = reader.GetFieldValue<string>("str");
            result.Integer = reader.GetFieldValue<int>("int");
            result.Decimal = reader.GetFieldValue<decimal>("dec");
            result.Bytes = reader.GetFieldValue<byte[]>("bytes");
            result.Float = reader.GetFieldValue<float>("float");
            result.Bool = reader.GetFieldValue<bool>("bool");
            result.Timestamp = reader.GetFieldValue<DateTime>("timestamp");
        }

        return result;
    }

    public struct CastDataType
    {
        public string String { get; set; }

        public int Integer { get; set; }

        public decimal Decimal { get; set; }

        public byte[] Bytes { get; set; }

        public float Float { get; set; }

        public bool Bool { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
// [END spanner_postgresql_cast_data_type]
