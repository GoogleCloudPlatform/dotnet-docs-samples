// Copyright 2024 Google Inc.
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

using Google.Cloud.Spanner.Admin.Instance.V1;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInstancePartitionTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateInstancePartitionTest(SpannerFixture spannerFixture) => _spannerFixture = spannerFixture;

    [Fact]
    public void TestCreateInstancePartition()
    {
        var instancePartitionId = SpannerFixture.GenerateId("my-prt-");
        var createInstancePartitionSample = new CreateInstancePartitionSample();

        InstancePartition partition = createInstancePartitionSample.CreateInstancePartition(
            _spannerFixture.ProjectId, _spannerFixture.InstanceIdWithInstancePartition, instancePartitionId);

        Assert.Equal(instancePartitionId, partition.InstancePartitionName.InstancePartitionId);
    }
}
