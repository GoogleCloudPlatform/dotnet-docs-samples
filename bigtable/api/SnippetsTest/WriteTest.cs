// Copyright (c) 2018 Google LLC.
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
using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;

namespace Writes
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
        }

        public void Dispose()
        {
            _bigtableTableAdminClient.DeleteTable(new Google.Cloud.Bigtable.Common.V2.TableName(projectId, instanceId, tableId));
        }
    }
    public class WriteSnippetsTest : IClassFixture<BigtableClientFixture>
    {
        private readonly BigtableClientFixture _fixture;

        public WriteSnippetsTest(BigtableClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestWriteSimpleIncrementConditional()
        {
            Writes.WriteSimpleSample writeSimple = new Writes.WriteSimpleSample();
            Assert.Contains("Successfully wrote row", writeSimple.WriteSimple(_fixture.projectId, _fixture.instanceId, _fixture.tableId));

            Writes.WriteIncrementSample writeIncrement = new Writes.WriteIncrementSample();
            Assert.Contains("Successfully updated row", writeIncrement.WriteIncrement(_fixture.projectId, _fixture.instanceId, _fixture.tableId));

            Writes.WriteConditionalSample writeConditional = new Writes.WriteConditionalSample();
            Assert.Contains("Successfully updated row's os_name: True", writeConditional.WriteConditional(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }

        [Fact]
        public void TestWriteBatch()
        {
            Writes.WriteBatchSample writeBatch = new Writes.WriteBatchSample();
            Assert.Contains("Successfully wrote 2 rows", writeBatch.WriteBatch(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
        }
    }
}
