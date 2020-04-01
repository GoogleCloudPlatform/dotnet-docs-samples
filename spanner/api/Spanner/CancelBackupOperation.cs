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
    public class CancelBackupOperation
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(CancelBackupOperation));

        // [START spanner_cancel_backup_create]
        public static object SpannerCancelBackupOperation(
            string projectId, string instanceId, string databaseId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Initialize request parameters.
            Backup backup = new Backup
            {
                Database = DatabaseName.Format(projectId, instanceId, databaseId),
                ExpireTime = DateTime.UtcNow.AddDays(14).ToTimestamp()
            };
            string parent = InstanceName.Format(projectId, instanceId);

            // Make the CreateBackup request.
            Operation<Backup, CreateBackupMetadata> response =
                databaseAdminClient.CreateBackup(parent, backup, backupId);

            // Create the OperationsClient instance and execute CancelOperation.
            OperationsClient operationsClient = OperationsClient.Create();
            operationsClient.CancelOperation(response.Name);

            s_logger.Info("Waiting for the operation to finish.");

            // Poll until the long-running operation is complete. It will
            // either complete or be cancelled.
            Operation<Backup, CreateBackupMetadata> completedResponse =
                response.PollUntilCompleted();

            if (!completedResponse.IsFaulted)
            {
                s_logger.Info("Delete backup because it completed before it could be cancelled.");
                databaseAdminClient.DeleteBackup(BackupName.Format(projectId, instanceId, backupId));
            }

            s_logger.Info($"Operation {response.Name} canceled.");
            return ExitCode.Success;
        }
        // [END spanner_cancel_backup_create]
    }
}
