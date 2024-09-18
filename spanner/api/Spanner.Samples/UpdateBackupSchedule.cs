// Copyright 2024 Google Inc.
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

// [START spanner_update_backup_schedule]
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateBackupScheduleSample
{
    public BackupSchedule UpdateBackupSchedule(string projectId, string instanceId, string databaseId, string scheduleId)
    {
        DatabaseAdminClient client = DatabaseAdminClient.Create();

        BackupSchedule response = client.UpdateBackupSchedule(
            new UpdateBackupScheduleRequest
            {
                BackupSchedule = new BackupSchedule
                {
                    BackupScheduleName = BackupScheduleName.FromProjectInstanceDatabaseSchedule(projectId, instanceId, databaseId, scheduleId),
                    Spec = new BackupScheduleSpec
                    {
                        CronSpec = new CrontabSpec
                        {
                            Text = "45 15 * * *",
                        },
                    },
                    RetentionDuration = new Duration
                    {
                        Seconds = 172800,
                    },
                    EncryptionConfig = new CreateBackupEncryptionConfig
                    {
                        EncryptionType = CreateBackupEncryptionConfig.Types.EncryptionType.UseDatabaseEncryption,
                    },
                },
                UpdateMask = new FieldMask
                {
                    Paths = { "spec.cron_spec.text", "retention_duration", "encryption_config" },
                },
            });

        Console.WriteLine($"Updated backup schedule: {response}");
        return response;
    }
}
// [END spanner_update_backup_schedule]
