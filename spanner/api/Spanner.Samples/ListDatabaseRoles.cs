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

// [START spanner_list_database_roles]

using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Database.V1;
using System;

public class ListDatabaseRolesSample
{
    public PagedEnumerable<ListDatabaseRolesResponse, DatabaseRole> ListDatabaseRoles(string projectId, string instanceId, string databaseId)
    {
        string parent = $"projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        var client = DatabaseAdminClient.Create();
        PagedEnumerable<ListDatabaseRolesResponse, DatabaseRole> databaseRoles = client.ListDatabaseRoles(parent);
        foreach (var dbRole in databaseRoles)
        {
            Console.WriteLine($"Database Role: {dbRole.DatabaseRoleName}");
        }
        return databaseRoles;
    }
}
// [END spanner_list_database_roles]
