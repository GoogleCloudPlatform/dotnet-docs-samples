// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START spanner_update_database]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

public class UpdateDatabaseAsyncSample
{
    public async Task<Database> UpdateDatabaseAsync(string projectId, string instanceId, string databaseId)
    {
        var databaseAdminClient = await DatabaseAdminClient.CreateAsync();
        var databaseName = DatabaseName.Format(projectId, instanceId, databaseId);
        Database databaseToUpdate = await databaseAdminClient.GetDatabaseAsync(databaseName);

        databaseToUpdate.EnableDropProtection = true;
        var updateDatabaseRequest = new UpdateDatabaseRequest()
        {
            Database = databaseToUpdate,
            UpdateMask = new FieldMask { Paths = { "enable_drop_protection" } }
        };

        var operation = await databaseAdminClient.UpdateDatabaseAsync(updateDatabaseRequest);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the operation to finish.");
        var completedResponse = await operation.PollUntilCompletedAsync();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while updating database {databaseId}: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Updated database {databaseId}.");

        // Return the updated database.
        return completedResponse.Result;
    }
}
// [END spanner_update_database]
