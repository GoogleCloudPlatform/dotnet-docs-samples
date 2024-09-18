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
public class UpdateBackupScheduleTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateBackupScheduleTest(SpannerFixture spannerFixture) => _spannerFixture = spannerFixture;

    [Fact]
    public void TestUpdateBackupSchedule()
    {
        var scheduleId = SpannerFixture.GenerateId("schedule-");
        Assert.NotNull(
            new CreateFullBackupScheduleSample()
                .CreateFullBackupSchedule(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, scheduleId)
        );

        var sample = new UpdateBackupScheduleSample();
        var schedule = sample.UpdateBackupSchedule(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, scheduleId);
        Assert.NotNull(schedule);

        new DeleteBackupScheduleSample()
            .DeleteBackupSchedule(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, scheduleId);
    }
}
