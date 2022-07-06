// Copyright 2022 Google Inc.
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

// [START spanner_list_copy_backup_operations]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using System;
using System.Collections.Generic;

public class ListCopyBackupOperationsSample
{
    public IEnumerable<Operation> ListCopyBackupOperations(string projectId, string instanceId, string databaseId, string backupId)
    {
        // Create the DatabaseAdminClient instance.
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        var filter = $"(metadata.@type:type.googleapis.com/google.spanner.admin.database.v1.CopyBackupMetadata) AND (metadata.source_backup:{backupId})";
        ListBackupOperationsRequest request = new ListBackupOperationsRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            Filter = filter
        };

        // List the copy backup operations on the database.
        var backupOperations = databaseAdminClient.ListBackupOperations(request);

        foreach (var operation in backupOperations)
        {
            CopyBackupMetadata metadata = operation.Metadata.Unpack<CopyBackupMetadata>();
            Console.WriteLine($"Backup {metadata.Name} from source backup {metadata.SourceBackup} is {metadata.Progress.ProgressPercent}% complete");
        }

        return backupOperations;
    }
}

// [END spanner_list_copy_backup_operations]
