// Copyright 2023 Google Inc.
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

// [START spanner_drop_sequence]
// [START spanner_postgresql_drop_sequence]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Threading.Tasks;

public class DropSequenceSample
{
    public async Task DropSequenceSampleAsync(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);
        string[] statements =
        {
            "ALTER TABLE Customers ALTER COLUMN CustomerId DROP DEFAULT",
            "DROP SEQUENCE Seq"
        };
        var operation = await databaseAdminClient.UpdateDatabaseDdlAsync(databaseName, statements);

        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            throw completedResponse.Exception;
        }
        Console.WriteLine("Altered Customers table to drop DEFAULT from CustomerId column and dropped the Seq sequence");
    }
}

// [END spanner_drop_sequence]
// [END spanner_postgresql_drop_sequence]
