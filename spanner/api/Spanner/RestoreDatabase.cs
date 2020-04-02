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
using Google.LongRunning;
using log4net;
using System;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class RestoreDatabase
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(RestoreDatabase));

        // [START spanner_restore_database]
        public static object SpannerRestoreDatabase(
            string projectId, string instanceId, string databaseId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            string parent = InstanceName.Format(projectId, instanceId);
            string backupName = BackupName.Format(projectId, instanceId, backupId);

            // Make the RestoreDatabase request.
            Operation<Database, RestoreDatabaseMetadata> response =
                databaseAdminClient.RestoreDatabase(parent, databaseId, backupName);

            Console.WriteLine("Waiting for the operation to finish");

            // Poll until the returned long-running operation is complete.
            var completedResponse = response.PollUntilCompleted();

            if (completedResponse.IsFaulted)
            {
                s_logger.Error($"Database Restore Failed: {completedResponse.Exception}");
                return ExitCode.InvalidParameter;
            }

            RestoreInfo restoreInfo = completedResponse.Result.RestoreInfo;
            s_logger.Info($"Database {restoreInfo.BackupInfo.SourceDatabase} " +
                          $"restored from backup {restoreInfo.BackupInfo.Backup}");

            return ExitCode.Success;
        }
        // [END spanner_restore_database]
    }
}
