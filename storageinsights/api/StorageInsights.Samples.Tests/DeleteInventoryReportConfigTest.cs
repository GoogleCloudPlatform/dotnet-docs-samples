// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageInsights.V1;
using GoogleCloudSamples;
using Xunit;

namespace StorageInsights.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class DeleteInventoryReportConfigTest
{
    private readonly StorageFixture _fixture;

    public DeleteInventoryReportConfigTest(StorageFixture fixture) => _fixture = fixture;

    [Fact]
    public void TestDeleteInventoryReportConfig()
    {
        RetryRobot robot = new RetryRobot() { ShouldRetry = (e) => true };
        robot.Eventually(() =>
        {
            var reportConfig = new CreateInventoryReportConfigSample().CreateInventoryReportConfig(_fixture.ProjectId,
                _fixture.BucketLocation, _fixture.BucketNameSource,
                _fixture.BucketNameSink);
            DeleteInventoryReportConfigSample sample = new DeleteInventoryReportConfigSample();
            sample.DeleteInventoryReportConfig(_fixture.ProjectId, _fixture.BucketLocation,
                reportConfig.Name.Split("/")[5]);

            foreach (ReportConfig report in _fixture.StorageInsights.ListReportConfigs(
                         LocationName.Format(_fixture.ProjectId, _fixture.BucketLocation)))
            {
                Assert.NotEqual(report.Name, reportConfig.Name);
            }
        });
    }
}
