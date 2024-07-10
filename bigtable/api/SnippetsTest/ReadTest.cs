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

namespace Reads_
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
            CreateTableRequest createTableRequest = new CreateTableRequest
            {
                ParentAsInstanceName = new InstanceName(projectId, instanceId),
                Table = table,
                TableId = tableId,
            };
            _bigtableTableAdminClient.CreateTable(createTableRequest);

            TableName tableName = new TableName(projectId, instanceId, tableId);
            BigtableVersion timestamp = new BigtableVersion(new DateTime(2020, 1, 10, 14, 0, 0, DateTimeKind.Utc));

            MutateRowsRequest.Types.Entry[] entries = {
            Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190501"),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.003", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190502"),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.004", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190505"),
                Mutations.SetCell("stats_summary", "connected_cell", "0", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190406.000", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190501"),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190401.002", timestamp)),
            Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190502"),
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

    public class ReadSnippetsTest : IClassFixture<BigtableClientFixture>
    {
        private readonly BigtableClientFixture _fixture;

        public ReadSnippetsTest(BigtableClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestReadRow()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(readSnippets.ReadRow(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public void TestReadRowPartial()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(readSnippets.ReadRowPartial(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestReadRowsAsync()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(await readSnippets.ReadRows(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestReadRowRangeAsync()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(await readSnippets.ReadRowRange(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestReadRowRangesAsync()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(await readSnippets.ReadRowRanges(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestReadPrefixAsync()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(await readSnippets.ReadPrefix(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public async Task TestReadFilter()

        {
            Reads.ReadSnippets readSnippets = new Reads.ReadSnippets();

            Snapshooter.Xunit.Snapshot.Match(await readSnippets.ReadFilter(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }
    }
}
