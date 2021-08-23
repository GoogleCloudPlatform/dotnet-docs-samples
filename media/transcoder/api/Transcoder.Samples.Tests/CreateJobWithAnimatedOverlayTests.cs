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

using System;
using System.Threading;
using Xunit;

namespace Transcoder.Samples.Tests
{
    [Collection(nameof(TranscoderFixture))]
    public class CreateJobWithAnimatedOverlayTest : IDisposable
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobWithAnimatedOverlaySample _createSample;
        private string _jobId;
        private readonly GetJobStateSample _getSample;
        private readonly DeleteJobSample _deleteSample;

        public CreateJobWithAnimatedOverlayTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobWithAnimatedOverlaySample();
            _getSample = new GetJobStateSample();
            _deleteSample = new DeleteJobSample();
        }

        [Fact]
        public void CreatesJobWithAnimatedOverlay()
        {
            string outputUri = "gs://" + _fixture.BucketName + "/test-output-animated-overlay/";
            // Run the sample code.
            var result = _createSample.CreateJobWithAnimatedOverlay(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, overlayImageUri: _fixture.OverlayImageUri, outputUri: outputUri);

            string jobName = string.Format("projects/{0}/locations/{1}/jobs/", _fixture.ProjectNumber, _fixture.Location);
            Assert.Contains(jobName, result);
            string[] arr = result.Split("/");
            _jobId = arr[arr.Length - 1].Replace("\n", "");

            string state = "";

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
                if (state.Contains(_fixture.JobStateSucceeded))
                {
                    break;
                }
            }
            Assert.Contains(_fixture.JobStateSucceeded, state);
        }

        public void Dispose()
        {
            _deleteSample.DeleteJob(projectId: _fixture.ProjectId, location: _fixture.Location, _jobId);
        }
    }
}
