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

// [START spanner_list_backups]

using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;

public class ListBackupsSample
{
    public IEnumerable<Backup> ListBackups(string projectId, string instanceId, string databaseId, string backupId)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        InstanceName parentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId);

        // List all backups.
        Console.WriteLine("All backups:");
        var allBackups = databaseAdminClient.ListBackups(parentAsInstanceName);
        PrintBackups(allBackups);

        ListBackupsRequest request = new ListBackupsRequest
        {
            ParentAsInstanceName = parentAsInstanceName,
        };

        // List backups containing backup name.
        Console.WriteLine($"Backups with backup name containing {backupId}:");
        request.Filter = $"name:{backupId}";
        var backupsWithName = databaseAdminClient.ListBackups(request);
        PrintBackups(backupsWithName);

        // List backups on a database containing name.
        Console.WriteLine($"Backups with database name containing {databaseId}:");
        request.Filter = $"database:{databaseId}";
        var backupsWithDatabaseName = databaseAdminClient.ListBackups(request);
        PrintBackups(backupsWithDatabaseName);

        // List backups that expire within 30 days.
        Console.WriteLine("Backups expiring within 30 days:");
        string expireTime = DateTime.UtcNow.AddDays(30).ToString("O");
        request.Filter = $"expire_time < \"{expireTime}\"";
        var expiringBackups = databaseAdminClient.ListBackups(request);
        PrintBackups(expiringBackups);

        // List backups with a size greater than 100 bytes.
        Console.WriteLine("Backups with size > 100 bytes:");
        request.Filter = "size_bytes > 100";
        var backupsWithSize = databaseAdminClient.ListBackups(request);
        PrintBackups(backupsWithSize);

        // List backups created in the last day that are ready.
        Console.WriteLine("Backups created within last day that are ready:");
        string createTime = DateTime.UtcNow.AddDays(-1).ToString("O");
        request.Filter = $"create_time >= \"{createTime}\" AND state:READY";
        var recentReadyBackups = databaseAdminClient.ListBackups(request);
        PrintBackups(recentReadyBackups);

        // List backups in pages.
        foreach (var page in databaseAdminClient.ListBackups(parentAsInstanceName, pageSize: 5).AsRawResponses())
        {
            PrintBackups(page);
        }

        return allBackups;
    }

    private static void PrintBackups(IEnumerable<Backup> backups)
    {
        foreach (Backup backup in backups)
        {
            Console.WriteLine($"Backup Name : {backup.Name}");
        };
    }
}
// [END spanner_list_backups]
