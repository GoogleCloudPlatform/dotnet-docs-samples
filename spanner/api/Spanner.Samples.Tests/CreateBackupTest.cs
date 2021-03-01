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

using Google.Cloud.Spanner.Data;
using Grpc.Core;
using System;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateBackupTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateBackupTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public void TestCreateBackup()
    {
        string connectionString = $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{_spannerFixture.BackupDatabaseId}";
        using var connection = new SpannerConnection(connectionString);
        connection.Open();
        var versionTime = (DateTime) connection.CreateSelectCommand("SELECT CURRENT_TIMESTAMP").ExecuteScalar();

        CreateBackupSample createBackupSample = new CreateBackupSample();
        // Backup already exists since it was created in the test setup so it should throw an exception.
        var exception = Assert.Throws<RpcException>(()
            => createBackupSample.CreateBackup(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.BackupDatabaseId, _spannerFixture.BackupId, versionTime));
        Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
    }
}