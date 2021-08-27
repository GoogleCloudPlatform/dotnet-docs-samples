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

using System.Threading;
using Xunit;
using Google.Cloud.Video.Transcoder.V1;

namespace Transcoder.Samples.Tests
{
    [Collection(nameof(TranscoderFixture))]
    public class CreateJobWithSetNumberImagesSpritesheetTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobWithSetNumberImagesSpritesheetSample _createSample;
        private string _jobId;
        private readonly GetJobStateSample _getSample;
        private readonly DeleteJobSample _deleteSample;

        public CreateJobWithSetNumberImagesSpritesheetTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobWithSetNumberImagesSpritesheetSample();
            _getSample = new GetJobStateSample();
            _deleteSample = new DeleteJobSample();
        }

        [Fact]
        public void CreatesJobWithSetNumberImagesSpritesheet()
        {
            string outputUri = "gs://" + _fixture.BucketName + "/test-output-set-number-spritesheet/";
            // Run the sample code.
            var result = _createSample.CreateJobWithSetNumberImagesSpritesheet(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, outputUri: outputUri);

            Assert.Equal(result.JobName.LocationId, _fixture.Location);
            // Job resource name uses project number for the identifier.
            Assert.Equal(result.JobName.ProjectId, _fixture.ProjectNumber);
            _jobId = result.JobName.JobId;
            _fixture.jobIds.Add(_jobId);

            Job.Types.ProcessingState state = Job.Types.ProcessingState.Unspecified;

            for (int attempt = 0; attempt < 5; attempt++)
            {
                Thread.Sleep(60000);
                try
                {
                    state = _getSample.GetJobState(projectId: _fixture.ProjectId, location: _fixture.Location, _jobId);
                }
                catch (Google.GoogleApiException e)
                when (e.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Job does not exist yet.  No problem.
                }
                if (state.Equals(_fixture.JobStateSucceeded))
                {
                    break;
                }
            }
            Assert.Equal(state, _fixture.JobStateSucceeded);
        }
    }
}
