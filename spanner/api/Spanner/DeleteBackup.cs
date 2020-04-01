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
    public class DeleteBackup
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(DeleteBackup));

        // [START spanner_delete_backup]
        public static object SpannerDeleteBackup(string projectId, string instanceId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Make the DeleteBackup request.
            databaseAdminClient.DeleteBackup(BackupName.Format(projectId, instanceId, backupId));

            s_logger.Info("Backup deleted successfully.");

            return ExitCode.Success;
        }
        // [END spanner_delete_backup]
    }
}
