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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using log4net;
using System.Linq;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class GetBackups
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(GetBackups));

        // [START spanner_get_backups]
        public static object SpannerGetBackups(string projectId, string instanceId, string backupId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            var listBackupRequest = new ListBackupsRequest
            {
                Parent = InstanceName.Format(projectId, instanceId),
                Filter = $"name:{backupId}"
            };

            // Make the ListBackups requests.
            var backups = databaseAdminClient.ListBackups(listBackupRequest).ToList();

            backups.ForEach(backup => {
                s_logger.Info($"Backup Name : {backup.Name}");
                s_logger.Info($"Backup Created Time : {backup.CreateTime}");
                s_logger.Info($"Backup Databasee : {backup.Database}");
                s_logger.Info($"Backup ExpireTime : {backup.ExpireTime}");
                s_logger.Info($"Backup State : {backup.State}");
            });

            return ExitCode.Success;
        }
        // [END spanner_get_backups]
    }
}
