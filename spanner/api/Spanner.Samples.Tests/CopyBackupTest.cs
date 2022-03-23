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

using Xunit;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;

[Collection(nameof(SpannerFixture))]
public class CopyBackupTest
{
    private readonly SpannerFixture _spannerFixture;

    public CopyBackupTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public void CopyBackup()
    {
        DatabaseAdminClient databaseAdminClient = new DatabaseAdminClientBuilder { Endpoint = "staging-wrenchworks.sandbox.googleapis.com" }.Build();

        CopyBackupSample copyBackupSample = new CopyBackupSample();
        string source_Project_id = _spannerFixture.ProjectId;
        string source_Instance_id = _spannerFixture.InstanceId;
        string source_backupId = _spannerFixture.BackupId;
        string target_Project_id = "test1";
        string target_Instance_id = "testinst1";
        string target_backupId = "backup_target6";

        DateTimeOffset expireTime = DateTimeOffset.UtcNow.AddDays(7);
        DateTimeOffset maxExpireTime = DateTimeOffset.Now.AddDays(10);

        copyBackupSample.CopyBackup(source_Instance_id, source_Project_id, source_backupId, target_Instance_id, target_Project_id, target_backupId, expireTime, maxExpireTime);

        BackupName backupName = BackupName.FromProjectInstanceBackup(target_Project_id, target_Instance_id, target_backupId);
        Backup backup = databaseAdminClient.GetBackup(backupName);
        Assert.NotNull(backup);
    }
}