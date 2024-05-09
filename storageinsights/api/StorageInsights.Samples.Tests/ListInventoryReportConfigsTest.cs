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
using System.Linq;
using Xunit;

namespace StorageInsights.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class ListInventoryReportConfigsTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _reportConfigName;

    public ListInventoryReportConfigsTest(StorageFixture fixture) => _fixture = fixture;

    [Fact]
    public void TestListInventoryReportConfigs()
    {
        // The permissions the service account needs sometimes needs a few minutes to propagate, so retry permission issues
        RetryRobot robot = new RetryRobot() { ShouldRetry = (e) => e.Message.Contains("PermissionDenied") };
        ReportConfig reportConfig = null;
        robot.Eventually(() =>
        {
            reportConfig = new CreateInventoryReportConfigSample().CreateInventoryReportConfig(_fixture.ProjectId,
                _fixture.BucketLocation, _fixture.BucketNameSource,
                _fixture.BucketNameSink);
            _reportConfigName = reportConfig.Name;
        });

        ListInventoryReportConfigsSample sample = new ListInventoryReportConfigsSample();
        var reportConfigs = sample.ListInventoryReportConfigs(_fixture.ProjectId, _fixture.BucketLocation);
        Assert.Contains(reportConfigs, config => config.Name.Contains(reportConfig.ReportConfigName.ReportConfigId));
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
