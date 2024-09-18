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

using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateIncrementalBackupScheduleTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateIncrementalBackupScheduleTest(SpannerFixture spannerFixture) => _spannerFixture = spannerFixture;

    [Fact]
    public void TestCreateIncrementalBackupSchedule()
    {
        var scheduleId = SpannerFixture.GenerateId("schedule-");
        var sample = new CreateIncrementalBackupScheduleSample();
        var schedule = sample.CreateIncrementalBackupSchedule(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, scheduleId);
        Assert.NotNull(schedule);

        new DeleteBackupScheduleSample()
            .DeleteBackupSchedule(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, scheduleId);
    }
}
