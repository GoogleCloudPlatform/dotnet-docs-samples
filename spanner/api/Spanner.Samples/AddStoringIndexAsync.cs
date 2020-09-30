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

// [START spanner_create_storing_index]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AddStoringIndexAsyncSample
{
    public async Task AddStoringIndexAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string createStatement = "CREATE INDEX AlbumsByAlbumTitle2 ON Albums(AlbumTitle) STORING (MarketingBudget)";

        using var connection = new SpannerConnection(connectionString);
        using var createCmd = connection.CreateDdlCommand(createStatement);
        await createCmd.ExecuteNonQueryAsync();
        Console.WriteLine("Added the AlbumsByAlbumTitle2 index.");
    }
}
// [END spanner_create_storing_index]
