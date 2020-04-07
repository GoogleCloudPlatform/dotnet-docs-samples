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

// [START spanner_get_backups]
using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloudSamples.Spanner
{
    public class GetBackups
    {
        public static object SpannerGetBackups(
            string projectId, string instanceId, string databaseId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            InstanceName parentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId);

            Action<List<Backup>> printBackups = backups =>
            {
                backups.ForEach(backup =>
                {
                    Console.WriteLine($"Backup Name : {backup.Name}");
                });
            };

            // List all backups.
            Console.WriteLine("All backups:");
            var allBackups = databaseAdminClient.ListBackups(parentAsInstanceName).ToList();
            printBackups(allBackups);

            // List backups containing backup name.
            Console.WriteLine($"Backups with backup name containing {backupId}:");
            var backupsWithName = databaseAdminClient.ListBackups(
                parentAsInstanceName, $"name:{backupId}").ToList();
            printBackups(backupsWithName);

            // List backups on a database containing name.
            Console.WriteLine($"Backups with database name containing {databaseId}:");
            var backupsWithDatabaseName = databaseAdminClient.ListBackups(
                parentAsInstanceName, $"database:{backupId}").ToList();
            printBackups(backupsWithDatabaseName);

            // List backups that expire within 30 days.
            Console.WriteLine("Backups expiring within 30 days:");
            var expireTime = DateTime.UtcNow.AddDays(30);
            var expiringBackups = databaseAdminClient.ListBackups(
                parentAsInstanceName, $"expire_time < {expireTime.ToString("O")}").ToList();
            printBackups(expiringBackups);

            // List backups with a size greater than 100 bytes.
            Console.WriteLine("Backups with size > 100 bytes:");
            var backupsWithSize = databaseAdminClient.ListBackups(
                parentAsInstanceName, "size_bytes > 100").ToList();
            printBackups(backupsWithSize);

            // List backups created in the last day that are ready.
            Console.WriteLine("Backups created within last day that are ready:");
            var createTime = DateTime.UtcNow.AddDays(-1);
            var recentReadyBackups = databaseAdminClient.ListBackups(
                parentAsInstanceName,
                $"create_time >= {createTime.ToString("O")} AND state:READY").ToList();
            printBackups(recentReadyBackups);

            // List backups in pages.
            Console.WriteLine("Backups in batches of 2:");
            int pageSize = 2;
            string nextPageToken = string.Empty;
            do
            {
                var request = new ListBackupsRequest
                {
                    ParentAsInstanceName = parentAsInstanceName,
                    PageToken = nextPageToken,
                };
                var response = databaseAdminClient.ListBackups(request);

                Page<Backup> currentPage = response.ReadPage(pageSize);
                foreach (Backup backup in currentPage)
                {
                    Console.WriteLine($"Backup Name : {backup.Name}");
                }

                nextPageToken = currentPage.NextPageToken;
            } while (!string.IsNullOrEmpty(nextPageToken));

            return 0;
        }
    }
}
// [END spanner_get_backups]
