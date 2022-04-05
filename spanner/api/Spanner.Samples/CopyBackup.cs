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

// [START spanner_copy_backup]

using Google.Api.Gax;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CopyBackupSample
{
    public Backup CopyBackup(string sourceInstanceId, string sourceProjectId, string sourceBackupId, 
        string targetInstanceId, string targetProjectId, string targetBackupId, 
        DateTimeOffset expireTime)
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        var request = new CopyBackupRequest
        {
            SourceBackupAsBackupName = new BackupName(sourceProjectId, sourceInstanceId, sourceBackupId), 
            ParentAsInstanceName = new InstanceName(targetProjectId, targetInstanceId),
            BackupId = targetBackupId,
            ExpireTime = Timestamp.FromDateTimeOffset(expireTime) 
        };

        var response = databaseAdminClient.CopyBackup(request);
        Console.WriteLine("Waiting for the operation to finish.");
        var completedResponse = response.PollUntilCompleted(new PollSettings(Expiration.FromTimeout(TimeSpan.FromMinutes(15)), TimeSpan.FromMinutes(2)));

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating backup: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Backup backup = completedResponse.Result;

        Console.WriteLine($"Backup created successfully.");
        Console.WriteLine($"Backup with Id {sourceBackupId} has been copied from {sourceProjectId}/{sourceInstanceId} to {targetProjectId}/{targetInstanceId} Backup {targetBackupId}");
        Console.WriteLine($"Backup {backup.Name} of size {backup.SizeBytes} bytes was created at {backup.CreateTime} from {backup.Database} and is in state {backup.State} and has version time {backup.VersionTime.ToDateTime()}");

        return backup;
    }
}

// [END spanner_copy_backup]