﻿// Copyright 2022 Google Inc.
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

using System.Linq;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class ListCopyBackupOperationsTest
{
    private readonly SpannerFixture _spannerFixture;

    public ListCopyBackupOperationsTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public void TestListCopyBackupOperations()
    {
        ListCopyBackupOperationsSample listCopyBackupOperationsSample = new ListCopyBackupOperationsSample();

        var operations = listCopyBackupOperationsSample.ListCopyBackupOperations(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.BackupDatabaseId,_spannerFixture.BackupId);
        Assert.NotNull(operations);
    }
}
