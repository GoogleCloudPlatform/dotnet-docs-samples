/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Xunit;

namespace Transcoder.Samples.Tests
{
    [Collection(nameof(TranscoderFixture))]
    public class ListJobsTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobFromAdHocSample _createSample;
        private readonly ListJobsSample _listSample;

        private string _jobId;

        public ListJobsTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobFromAdHocSample();
            _listSample = new ListJobsSample();

            string outputUri = $"gs://{_fixture.BucketName}/test-output-list-jobs/";
            // Run the sample code.
            var result = _createSample.CreateJobFromAdHoc(
                _fixture.ProjectId, _fixture.Location,
                _fixture.InputUri, outputUri);

            Assert.Equal(_fixture.Location, result.JobName.LocationId);
            // Job resource name uses project number for the identifier.
            Assert.Equal(_fixture.ProjectNumber, result.JobName.ProjectId);
            _jobId = result.JobName.JobId;
            _fixture.JobIds.Add(_jobId);
        }

        [Fact]
        public void ListsJobs()
        {
            var jobs = _listSample.ListJobs(_fixture.ProjectId, _fixture.Location);
            string jobName = string.Format("projects/{0}/locations/{1}/jobs/{2}", _fixture.ProjectNumber, _fixture.Location, _jobId);
            Assert.Contains(jobName, jobs);
        }
    }
}
