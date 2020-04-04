﻿// Copyright 2020 Google Inc.
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

// [START spanner_delete_backup]
using Google.Cloud.Spanner.Admin.Database.V1;
using System;

namespace GoogleCloudSamples.Spanner
{
    public class DeleteBackup
    {
        public static object SpannerDeleteBackup(string projectId, string instanceId, string backupId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            // Make the DeleteBackup request.
            databaseAdminClient.DeleteBackup(BackupName.Format(projectId, instanceId, backupId));

            Console.WriteLine("Backup deleted successfully.");

            return 0;
        }
    }
}
// [END spanner_delete_backup]
