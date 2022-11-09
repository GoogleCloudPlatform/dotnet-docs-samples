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

// [START spanner_read_data_with_database_role]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReadDataWithDatabaseRoleSample
{
    public class Venue
    {
        public int VenueId;

        public string VenueName;
    }

    public async Task<List<Venue>> ReadDataWithDatabaseRoleAsync(string projectId, string instanceId, string databaseId, string databaseRole)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string tableName = "Venues";

        var spannerConnectionStringBuilder = new SpannerConnectionStringBuilder
        {
            ConnectionString = connectionString,
            DatabaseRole = databaseRole
        };
        using var connection = new SpannerConnection(spannerConnectionStringBuilder);
        var createSelectCmd = connection.CreateSelectCommand($"SELECT * FROM {tableName}");
        using var reader = await createSelectCmd.ExecuteReaderAsync();
        var venues = new List<Venue>();
        while (reader.Read())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                VenueName = reader.GetFieldValue<string>("VenueName")
            });
        }
        return venues;
    }
}
// [END spanner_read_data_with_database_role]
