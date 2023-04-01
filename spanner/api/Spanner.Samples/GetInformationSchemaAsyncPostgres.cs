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

// [START spanner_postgresql_information_schema]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetInformationSchemaAsyncPostgresSample
{
    public async Task<List<InformationSchema>> GetInformationSchemaAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Get all the user tables in the database. PostgreSQL uses the `public` schema for user tables.
        var command = connection.CreateSelectCommand("SELECT table_schema, table_name, " +
            // The following columns are only available for PostgreSQL databases.
            "user_defined_type_catalog, " +
            "user_defined_type_schema, " +
            "user_defined_type_name " +
            "FROM INFORMATION_SCHEMA.tables " +
            "WHERE table_schema='public'");

        using var reader = await command.ExecuteReaderAsync();
        var result = new List<InformationSchema>();
        while (await reader.ReadAsync())
        {
            var informationSchema = new InformationSchema
            {
                Schema = reader.GetFieldValue<string>("table_schema"),
                Name = reader.GetFieldValue<string>("table_name")
            };

            var userDefinedTypeCatalog = reader.IsDBNull(reader.GetOrdinal("user_defined_type_catalog")) ? null
                : reader.GetFieldValue<string>("user_defined_type_catalog");
            var userDefinedTypeSchema = reader.IsDBNull(reader.GetOrdinal("user_defined_type_schema")) ? null
                : reader.GetFieldValue<string>("user_defined_type_schema");
            var userDefinedTypeName = reader.IsDBNull(reader.GetOrdinal("user_defined_type_name")) ? null
                : reader.GetFieldValue<string>("user_defined_type_name");

            informationSchema.UserDefinedType = string.IsNullOrWhiteSpace(userDefinedTypeName) ? "undefined"
                : $"{userDefinedTypeCatalog}.{userDefinedTypeSchema}.{userDefinedTypeName}";

            result.Add(informationSchema);
        }

        return result;
    }

    public struct InformationSchema
    {
        public string Schema { get; set; }

        public string Name { get; set; }

        public string UserDefinedType { get; set; }
    }
}
// [END spanner_postgresql_information_schema]
