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
using System.Linq;
using Xunit;
using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Admin.V2;
using Snapshooter;
using System.Threading.Tasks;

namespace Filter
{
    public class BigtableClientFixture : IDisposable
    {
        public readonly string projectId =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public readonly string instanceId =
            Environment.GetEnvironmentVariable("TEST_BIGTABLE_INSTANCE");
        public readonly string tableId = $"mobile-time-series-{Guid.NewGuid().ToString().Substring(0, 8)}";

        private readonly BigtableTableAdminClient _bigtableTableAdminClient;

        public BigtableClientFixture()
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            _bigtableTableAdminClient = BigtableTableAdminClient.Create();
            Table table = new Table
            {
                Granularity = Table.Types.TimestampGranularity.Millis
            };
            table.ColumnFamilies.Add("stats_summary", new ColumnFamily());
            table.ColumnFamilies.Add("cell_plan", new ColumnFamily());
            CreateTableRequest createTableRequest = new CreateTableRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId),
                Table = table,
                TableId = tableId,
            };
            _bigtableTableAdminClient.CreateTable(createTableRequest);

            TableName tableName = new TableName(projectId, instanceId, tableId);
            BigtableVersion timestamp = new BigtableVersion(new DateTime(2020, 1, 10, 14, 0, 0, DateTimeKind.Utc));
            BigtableVersion timestamp_minus_hr = new BigtableVersion(new DateTime(2020, 1, 10, 13, 0, 0, DateTimeKind.Utc));

            MutateRowsRequest.Types.Entry[] entries = {
            Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190501"),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "false", timestamp),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "true", timestamp_minus_hr),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.003", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190502"),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.004", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190505"),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "0", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190406.000", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190501"),
                Mutations.SetCell("cell_plan", "data_plan_10gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190401.002", timestamp)),
            Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190502"),
                Mutations.SetCell("cell_plan", "data_plan_10gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "0", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190406.000", timestamp))
                };

            bigtableClient.MutateRows(tableName, entries);
        }

        public void Dispose()
        {
            _bigtableTableAdminClient.DeleteTable(new Google.Cloud.Bigtable.Common.V2.TableName(projectId, instanceId, tableId));
        }
    }

    public class FilterSnippetsTest : IClassFixture<BigtableClientFixture>
    {
        private readonly BigtableClientFixture _fixture;

        public FilterSnippetsTest(BigtableClientFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async Task TestFilterLimitRowSampleAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Assert.Contains("Reading data for", await filterSnippets.FilterLimitRowSample(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitRowRegexAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitRowRegex(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitCellsPerColAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitCellsPerCol(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitCellsPerRowAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitCellsPerRow(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitColRangeAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitColRange(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitValueRangeAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitValueRange(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitValueRegexAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitValueRegex(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitBlockAllAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitBlockAll(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterLimitPassAllAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterLimitPassAll(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterModifyStripValueAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterModifyStripValue(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterModifyApplyLabelAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterModifyApplyLabel(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterComposingChainAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterComposingChain(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestFilterComposingConditionAsync()
        {
            Filters.FilterSnippets filterSnippets = new Filters.FilterSnippets();
            Snapshooter.Xunit.Snapshot.Match(await filterSnippets.FilterComposingCondition(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }
    }
}
