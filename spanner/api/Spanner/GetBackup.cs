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

namespace GoogleCloudSamples.Spanner
{
    public class GetBackup
    {
        // [START spanner_get_backup]
        public static Backup SpannerGetBackup(string projectId, string instanceId, string backupId)
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
            return backup;
        }
        // [END spanner_get_backup]
    }
}
