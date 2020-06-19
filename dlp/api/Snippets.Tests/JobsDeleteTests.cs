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

using Google.Cloud.Dlp.V2;
using System;
using System.IO;
using Xunit;

namespace GoogleCloudSamples
{
    public class JobsDeleteTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture Fixture { get; }
        public JobsDeleteTests(DlpTestFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestDeleteJob()
        {
            using var randomBucketFixture = new RandomBucketFixture();
            using var bucketCollector = new BucketCollector(randomBucketFixture.BucketName);
            var bucketName = randomBucketFixture.BucketName;
            var fileName = Guid.NewGuid().ToString();
            var objectName = $"gs://{bucketName}/{fileName}";
            bucketCollector.CopyToBucket(Path.Combine(Fixture.ResourcePath, "dates-input.csv"), fileName);
            var job = JobsCreate.CreateJob(Fixture.ProjectId, objectName);
            JobsDelete.DeleteJob(job.Name);
            var activeJobs = JobsList.ListDlpJobs(Fixture.ProjectId, "state = RUNNING", DlpJobType.InspectJob);
            Assert.DoesNotContain(activeJobs, j => j.Name == job.Name);
        }
    }
}
