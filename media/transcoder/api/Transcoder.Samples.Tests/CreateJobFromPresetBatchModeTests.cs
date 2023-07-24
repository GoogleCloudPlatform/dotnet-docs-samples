/*
 * Copyright 2023 Google LLC
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
    public class CreateJobFromPresetBatchModeTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobFromPresetBatchModeSample _createSample;
        private readonly GetJobStateSample _getSample;

        public CreateJobFromPresetBatchModeTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobFromPresetBatchModeSample();
            _getSample = new GetJobStateSample();
        }

        [Fact]
        public void CreatesJobFromPresetBatchMode()
        {
            string outputUri = $"gs://{_fixture.BucketName}/test-output-preset-batch-mode/";
            string preset = "preset/web-hd";
            // Run the sample code.
            var result = _createSample.CreateJobFromPresetBatchMode(
                _fixture.ProjectId, _fixture.Location,
                _fixture.InputUri, outputUri, preset);
            _fixture.JobIds.Add(result.JobName.JobId);

            Assert.Equal(_fixture.Location, result.JobName.LocationId);
            // Job resource name uses project number for the identifier.
            Assert.Equal(_fixture.ProjectNumber, result.JobName.ProjectId);

            _fixture.JobPoller.Eventually(() =>
                 Assert.Equal(_fixture.JobStateSucceeded, _getSample.GetJobState(_fixture.ProjectId, _fixture.Location, result.JobName.JobId)
            ));
        }
    }
}
