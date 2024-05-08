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

using Google.Cloud.StorageInsights.V1;
using GoogleCloudSamples;
using System;
using Xunit;

namespace StorageInsights.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class GetInventoryReportNamesTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _reportConfigName;

    public GetInventoryReportNamesTest(StorageFixture fixture) => _fixture = fixture;

    [Fact]
    public void TestGetInventoryReportNames()
    {
        ReportConfig reportConfig = null;
        // The permissions the service account needs sometimes needs a few minutes to propagate, so retry permission issues
        RetryRobot robot = new RetryRobot() { ShouldRetry = (e) => e.Message.Contains("PermissionDenied") };
        robot.Eventually(() =>
        {
            reportConfig = new CreateInventoryReportConfigSample().CreateInventoryReportConfig(_fixture.ProjectId,
                _fixture.BucketLocation, _fixture.BucketNameSource,
                _fixture.BucketNameSink);
            _reportConfigName = reportConfig.Name;
        });
        GetInventoryReportNamesSample sample = new GetInventoryReportNamesSample();
        sample.GetInventoryReportNames(_fixture.ProjectId, _fixture.BucketLocation,
            reportConfig.ReportConfigName.ReportConfigId);
        /* We can't actually test for a report config name showing up here, because we create
         * the bucket and inventory configs for this test, and it takes 24 hours for an
         * inventory report to actually get written to the bucket.
         * We could set up a hard-coded bucket, but that would probably introduce flakes.
         * The best we can do is make sure the test runs without throwing an error
         */
    }

    public void Dispose()
    {
        try
        {
            _fixture.StorageInsights.DeleteReportConfig(_reportConfigName);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
