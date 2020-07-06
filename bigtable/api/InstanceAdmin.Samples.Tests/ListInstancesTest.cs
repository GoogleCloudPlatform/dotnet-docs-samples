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

[Collection(nameof(BigtableInstanceAdminFixture))]
public class ListInstancesTest
{
    private readonly BigtableInstanceAdminFixture _fixture;
    public ListInstancesTest(BigtableInstanceAdminFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestListInstances()
    {
        ListInstancesSample listInstancesSample = new ListInstancesSample();
        var instances = listInstancesSample.ListInstances(_fixture.ProjectId);
        Assert.Contains(instances, c => c.InstanceName.InstanceId.Contains(_fixture.InstanceId));
    }
}
