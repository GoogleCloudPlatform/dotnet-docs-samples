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

// Created job: { "name": "projects/dlapi-test/dlpJobs/i-4395928244510149959", "type": "INSPECT_JOB", "state": "PENDING", "inspectDetails": { "requestedOptions": { "snapshotInspectTemplate": { }, "jobConfig": { "storageConfig": { "cloudStorageOptions": { "fileSet": { "url": "gs://frucanpzcniuzunxeqdt/c8a9671e-7a0b-436f-8f82-815f75cc2850" } }, "timespanConfig": { "enableAutoPopulationOfTimespanConfig": true } }, "inspectConfig": { "infoTypes": [ { "name": "EMAIL_ADDRESS" }, { "name": "CREDIT_CARD_NUMBER" } ], "minLikelihood": "UNLIKELY", "limits": { "maxFindingsPerItem": 100 }, "includeQuote": true } } }, "result": { } }, "createTime": "2020-06-11T21:24:34.436Z" }
