﻿// Copyright 2021 Google Inc.
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

// [START spanner_get_database_ddl]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetDatabaseDdlAsyncSample
{
    public async Task<List<string>> GetDatabaseDdlAsync(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();
        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);
        var databaseDdl = await databaseAdminClient.GetDatabaseDdlAsync(databaseName);

        var ddlStatements = new List<string>();
        Console.WriteLine($"DDL statements for database {databaseId}:");
        foreach (var statement in databaseDdl.Statements)
        {
            Console.WriteLine(statement);
            ddlStatements.Add(statement);
        }

        return ddlStatements;
    }
}
// [END spanner_get_database_ddl]
