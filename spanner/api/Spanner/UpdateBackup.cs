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
using Google.Protobuf.WellKnownTypes;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class UpdateBackup
    {
        // [START spanner_update_backup]
        public static Backup SpannerUpdateBackup(string projectId, string instanceId, string backupId)
        {
            // Create the Database Admin Client instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            UpdateBackupRequest backupUpdateRequest = new UpdateBackupRequest
            {
                UpdateMask = new FieldMask()
                {
                    Paths =
                    {
                        "expire_time"
                    }
                },
                Backup = new Backup
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1).ToTimestamp(),
                    BackupName = new BackupName(projectId, instanceId, backupId),
                }
            };

            // Make the UpdateBackup requests.
            var updatedBackup = databaseAdminClient.UpdateBackup(backupUpdateRequest);
            return updatedBackup;
        }
        // [END spanner_update_backup]
    }
}
