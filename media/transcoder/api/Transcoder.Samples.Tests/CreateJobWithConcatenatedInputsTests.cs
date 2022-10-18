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
using Xunit;

namespace Transcoder.Samples.Tests
{
    [Collection(nameof(TranscoderFixture))]
    public class CreateJobWithConcatenatedInputsTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobWithConcatenatedInputsSample _createSample;
        private readonly GetJobStateSample _getSample;

        public CreateJobWithConcatenatedInputsTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobWithConcatenatedInputsSample();
            _getSample = new GetJobStateSample();
        }

        [Fact]
        public void CreatesJobWithConcatenatedInputs()
        {
            string outputUri = $"gs://{_fixture.BucketName}/test-output-concat/";
            // Run the sample code.
            var result = _createSample.CreateJobWithConcatenatedInputs(
                _fixture.ProjectId, _fixture.Location,
                _fixture.InputConcat1Uri, new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 8, 100),
                _fixture.InputConcat2Uri, new TimeSpan(0, 0, 0, 3, 500), new TimeSpan(0, 0, 0, 15, 0),
                outputUri);
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
