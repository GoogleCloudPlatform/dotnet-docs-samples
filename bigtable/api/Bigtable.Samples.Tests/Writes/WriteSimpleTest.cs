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

using Google.Cloud.Bigtable.Common.V2;
using System.Linq;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class WriteSimpleTest
{
    private readonly BigtableClientFixture _fixture;

    public WriteSimpleTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }
    [Fact]
    public void WriteSimple()
    {
        var writeSimple = new WriteSimple();
        ReadRowSample readRowSample = new ReadRowSample();
        writeSimple.Write(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId);
        var newlyCreatedRow = readRowSample.ReadRow(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId, "phone#4c410523#20190501");
        var cell = newlyCreatedRow.Families.Where(c => c.Name == "stats_summary").FirstOrDefault().Columns.Where(c => c.Qualifier.ToStringUtf8() == "connected_cell").FirstOrDefault().Cells[0];
        BigtableByteString value = cell.Value;
        Assert.Equal(1, (long)value);
    }
}
