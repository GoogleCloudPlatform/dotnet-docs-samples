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

// [START spanner_list_databases]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;

public class ListDatabasesSample
{
    public IEnumerable<Database> ListDatabases(string projectId, string instanceId)
    {
        var databaseAdminClient = DatabaseAdminClient.Create();
        var instanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        var databases = databaseAdminClient.ListDatabases(instanceName);

        foreach (var database in databases)
        {
            Console.WriteLine($"Default leader for database {database.DatabaseName.DatabaseId}: {database.DefaultLeader}");
        }

        return databases;
    }
}
// [END spanner_list_databases]
