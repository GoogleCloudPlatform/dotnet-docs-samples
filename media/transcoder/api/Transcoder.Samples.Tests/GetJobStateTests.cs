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
using Google.Cloud.Video.Transcoder.V1;

namespace Transcoder.Samples.Tests
{
    [Collection(nameof(TranscoderFixture))]
    public class GetJobStateTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobFromAdHocSample _createSample;
        private readonly GetJobStateSample _getSample;

        private string _jobId;

        public GetJobStateTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobFromAdHocSample();
            _getSample = new GetJobStateSample();

            string outputUri = $"gs://{_fixture.BucketName}/test-output-get-job/";
            var result = _createSample.CreateJobFromAdHoc(
                _fixture.ProjectId, _fixture.Location,
                _fixture.InputUri, outputUri);
            _jobId = result.JobName.JobId;
            _fixture.JobIds.Add(_jobId);
        }

        [Fact]
        public void GetsJobState()
        {
            // Run the sample code.
            Job.Types.ProcessingState result = _getSample.GetJobState(
                _fixture.ProjectId, _fixture.Location,
                _jobId);

            // Just to make the intention of this sample very explicit, which is to
            // return the job state.
            Assert.IsType<Job.Types.ProcessingState>(result);
        }
    }
}