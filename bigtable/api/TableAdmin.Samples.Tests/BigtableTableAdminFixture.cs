// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using Xunit;

[CollectionDefinition(nameof(BigtableTableAdminFixture))]
public class BigtableTableAdminFixture : IDisposable, ICollectionFixture<BigtableTableAdminFixture>
{
    public string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public string InstanceId { get; private set; } = Environment.GetEnvironmentVariable("TEST_BIGTABLE_INSTANCE") ?? "my-instance";
    public string TableId { get; private set; } = $"my-table-{Guid.NewGuid().ToString().Substring(0, 8)}";

    public BigtableTableAdminFixture()
    {
        CreateTableSample createTableSample = new CreateTableSample();
        createTableSample.CreateTable(ProjectId, InstanceId, TableId);
    }

    public void Dispose()
    {
        DeleteTableSample deleteTableSample = new DeleteTableSample();
        deleteTableSample.DeleteTable(ProjectId, InstanceId, TableId);
    }
}
