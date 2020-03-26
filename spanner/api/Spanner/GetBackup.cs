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
using log4net;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class GetBackup
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(GetBackup));

        // [START spanner_get_backup]
        public static object SpannerGetBackup(string projectId, string instanceId, string backupId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            //Create get backup request instance.
            GetBackupRequest getBackupRequest = new GetBackupRequest
            {
                BackupName = new BackupName(projectId, instanceId, backupId)
            };

            // Make the GetBackup request.
            Backup backup = databaseAdminClient.GetBackup(getBackupRequest);

            s_logger.Info($"Backup Name : {backup.Name}");
            s_logger.Info($"Backup Created Time : {backup.CreateTime}");
            s_logger.Info($"Backup Databasee : {backup.Database}");
            s_logger.Info($"Backup ExpireTime : {backup.ExpireTime}");
            s_logger.Info($"Backup State : {backup.State}");

            return ExitCode.Success;
        }
        // [END spanner_get_backup]
    }
}
