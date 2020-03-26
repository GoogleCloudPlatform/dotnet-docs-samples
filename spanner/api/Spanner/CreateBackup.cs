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
using Google.Protobuf.WellKnownTypes;
using log4net;
using System;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class CreateBackup
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(CreateBackup));

        // [START spanner_create_backup]
        public static object SpannerCreateBackup(string projectId, string instanceId, string databaseId,
            string backupId, string parentInstanceId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Initialize Create Backup Request instance.
            var backupRequest = new CreateBackupRequest
            {
                Backup = new Backup
                {
                    Database = DatabaseName.Format(projectId, instanceId, databaseId),
                    ExpireTime = DateTime.UtcNow.AddDays(1).ToTimestamp()
                },
                BackupId = backupId,
                Parent = InstanceName.Format(projectId, parentInstanceId)
            };

            // Make the CreateBackup request.
            Operation<Backup, CreateBackupMetadata> response =
                databaseAdminClient.CreateBackup(backupRequest);

            s_logger.Info("Waiting for the operation to finish.");

            // Poll until the returned long-running operation is complete.
            Operation<Backup, CreateBackupMetadata> completedResponse =
                response.PollUntilCompleted();

            if (!completedResponse.IsFaulted)
            {
                s_logger.Info($"Backup Created Successfully.");
                return ExitCode.Success;
            }
            else
            {
                s_logger.Error($"Error while creating backup: {completedResponse.Exception}");
                return ExitCode.InvalidParameter;
            }
        }
        // [END spanner_create_backup]
    }
}
