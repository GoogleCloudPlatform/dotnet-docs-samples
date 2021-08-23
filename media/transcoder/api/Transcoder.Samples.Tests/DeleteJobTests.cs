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
    public class DeleteJobTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobFromAdHocSample _createSample;
        private readonly DeleteJobSample _deleteSample;

        private string _jobId;

        public DeleteJobTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobFromAdHocSample();
            _deleteSample = new DeleteJobSample();

            string outputUri = "gs://" + _fixture.BucketName + "/test-output-delete-job/";
            // Run the sample code.
            var result = _createSample.CreateJobFromAdHoc(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, outputUri: outputUri);

            string[] arr = result.Split("/");
            _jobId = arr[arr.Length - 1].Replace("\n", "");
        }

        [Fact]
        public void DeletesJob()
        {
            // Run the sample code.
            var result = _deleteSample.DeleteJob(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                jobId: _jobId);

            Assert.Contains("Deleted job", result);
        }
    }
}
