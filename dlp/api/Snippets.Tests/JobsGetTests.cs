﻿// Copyright 2021 Google Inc.
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

using Google.Cloud.Dlp.V2;
using Xunit;

namespace GoogleCloudSamples
{
    public class JobsGetTests : IClassFixture<DlpTestFixture>
    {
        private RetryRobot TestRetryRobot { get; } = new RetryRobot();
        private DlpTestFixture Fixture { get; }
        public JobsGetTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestGetDlpJob()
        {
            var dlp = DlpServiceClient.Create();
            var dlpJob = dlp.CreateDlpJob(Fixture.GetTestRiskAnalysisJobRequest());

            TestRetryRobot.ShouldRetry = ex => true;
            TestRetryRobot.Eventually(() =>
            {
                Assert.Equal(dlpJob, JobsGet.GetDlpJob(dlpJob.Name));
            });
        }
    }
}
