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
using Google.Cloud.Spanner.Common.V1;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInstanceWithoutDefaultBackupSchedulesTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateInstanceWithoutDefaultBackupSchedulesTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestCreateInstanceAsync()
    {
        CreateInstanceWithoutDefaultBackupSchedulesAsyncSample createInstanceSample =
            new CreateInstanceWithoutDefaultBackupSchedulesAsyncSample();
        
        var instanceId = SpannerFixture.GenerateId("default-schedule-test-");
        Instance instance = await createInstanceSample.CreateInstanceWithoutDefaultBackupSchedulesAsync(
            _spannerFixture.ProjectId, instanceId);
       
        Assert.Equal(Instance.Types.DefaultBackupScheduleType.None, instance.DefaultBackupScheduleType);

        await _spannerFixture.InstanceAdminClient.DeleteInstanceAsync(
            InstanceName.FromProjectInstance(_spannerFixture.ProjectId, instanceId));
    }
}
