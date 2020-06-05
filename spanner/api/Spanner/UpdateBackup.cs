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

// [START spanner_update_backup]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Protobuf.WellKnownTypes;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class UpdateBackup
    {
        public static object SpannerUpdateBackup(string projectId, string instanceId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Retrieve existing backup.
            BackupName backupName =
                BackupName.FromProjectInstanceBackup(projectId, instanceId, backupId);
            Backup backup = databaseAdminClient.GetBackup(backupName);

            // Add 1 hour to the existing ExpireTime.
            backup.ExpireTime = backup.ExpireTime.ToDateTime().AddHours(1).ToTimestamp();

            UpdateBackupRequest backupUpdateRequest = new UpdateBackupRequest
            {
                UpdateMask = new FieldMask()
                {
                    Paths =
                    {
                        "expire_time"
                    }
                },
                Backup = backup
            };

            // Make the UpdateBackup requests.
            var updatedBackup = databaseAdminClient.UpdateBackup(backupUpdateRequest);

            Console.WriteLine($"Updated Backup ExpireTime: {updatedBackup.ExpireTime}");

            return 0;
        }
    }
}
// [END spanner_update_backup]
