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

// [START spanner_update_database_with_default_leader]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;
using Google.Cloud.Spanner.Common.V1;

public class UpdateDatabaseWithDefaultLeaderAsyncSample
{
    public async Task UpdateDatabaseWithDefaultLeaderAsync(string projectId, string instanceId, string databaseId, string defaultLeader)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();
        
        var alterDatabaseStatement = @$"ALTER DATABASE `{databaseId}` SET OPTIONS
                   (default_leader = '{defaultLeader}')";
        
        // Create the UpdateDatabaseDdl request and execute it.
        var request = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId),
            Statements = { alterDatabaseStatement }
        };
        var operation = await databaseAdminClient.UpdateDatabaseDdlAsync(request);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the operation to finish.");
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while updating database: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }
        Console.WriteLine("Updated default leader");
    }
}
// [END spanner_update_database_with_default_leader]
