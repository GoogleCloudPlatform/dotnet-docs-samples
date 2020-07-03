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

using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class ReadRowsTest
{
    private readonly BigtableClientFixture _fixture;

    public ReadRowsTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void TestReadRows()
    {
        ReadRowsSample readRowsSample = new ReadRowsSample();
        var result = await readRowsSample.ReadRowsAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId);
        Snapshooter.Xunit.Snapshot.Match(_fixture.GetRowsData(result));
    }
}
