// Copyright 2020 Google Inc.
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

using Google.Api.Gax;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class JobsCreateTests : IClassFixture<DlpTestFixture>
    {
        private RetryRobot TestRetryRobot { get; } = new RetryRobot();
        private DlpTestFixture Fixture { get; }
        public JobsCreateTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestCreateDlpJob()
        {
            using RandomBucketFixture randomBucketFixture = new RandomBucketFixture();
            using BucketCollector bucketCollector = new BucketCollector(randomBucketFixture.BucketName);
            string bucketName = randomBucketFixture.BucketName;
            string fileName = Guid.NewGuid().ToString();
            string objectName = $"gs://{bucketName}/{fileName}";
            bucketCollector.CopyToBucket(Path.Combine(Fixture.ResourcePath, "dates-input.csv"), fileName);
            Google.Cloud.Dlp.V2.DlpJob job = JobsCreate.CreateJob(Fixture.ProjectId, objectName);

            TestRetryRobot.ShouldRetry = ex => true;
            TestRetryRobot.Eventually(() =>
            {
                PagedEnumerable<Google.Cloud.Dlp.V2.ListDlpJobsResponse, Google.Cloud.Dlp.V2.DlpJob> response = JobsList.ListDlpJobs(Fixture.ProjectId, "state=DONE", "InspectJob");

                Assert.True(response.Any());
            });
        }
    }
}
