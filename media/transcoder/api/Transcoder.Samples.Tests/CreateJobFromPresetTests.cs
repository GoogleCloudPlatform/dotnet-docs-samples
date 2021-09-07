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
    public class CreateJobFromPresetTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobFromPresetSample _createSample;
        private readonly GetJobStateSample _getSample;

        public CreateJobFromPresetTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobFromPresetSample();
            _getSample = new GetJobStateSample();
        }

        [Fact]
        public void CreatesJobFromPreset()
        {
            string outputUri = "gs://" + _fixture.BucketName + "/test-output-preset/";
            string preset = "preset/web-hd";
            // Run the sample code.
            var result = _fixture.TranscoderChangesPropagated.Eventually(() => _createSample.CreateJobFromPreset(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, outputUri: outputUri, preset: preset));

            Assert.Equal(_fixture.Location, result.JobName.LocationId);
            // Job resource name uses project number for the identifier.
            Assert.Equal(_fixture.ProjectNumber, result.JobName.ProjectId);
            _fixture.jobIds.Add(result.JobName.JobId);

            _fixture.TranscoderChangesPropagated.Eventually(() =>
                 Assert.Equal(_fixture.JobStateSucceeded, _getSample.GetJobState(_fixture.ProjectId, _fixture.Location, result.JobName.JobId)
            ));
        }
    }
}
