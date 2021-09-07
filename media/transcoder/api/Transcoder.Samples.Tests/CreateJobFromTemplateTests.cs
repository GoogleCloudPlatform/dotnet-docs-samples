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
    public class CreateJobFromTemplateTest
    {
        private TranscoderFixture _fixture;
        private string _templateId;
        private readonly CreateJobTemplateSample _createTemplateSample;
        private readonly CreateJobFromTemplateSample _createJobSample;
        private readonly GetJobStateSample _getJobSample;

        public CreateJobFromTemplateTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createTemplateSample = new CreateJobTemplateSample();
            _createJobSample = new CreateJobFromTemplateSample();
            _getJobSample = new GetJobStateSample();
            _templateId = "my-job-template-" + _fixture.RandomId();

            _fixture.TranscoderChangesPropagated.Eventually(() => _createTemplateSample.CreateJobTemplate(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                templateId: _templateId));
        }

        [Fact]
        public void CreatesJobFromTemplate()
        {
            string outputUri = "gs://" + _fixture.BucketName + "/test-output-template/";
            // Run the sample code.
            var result = _fixture.TranscoderChangesPropagated.Eventually(() => _createJobSample.CreateJobFromTemplate(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, outputUri: outputUri, templateId: _templateId));

            Assert.Equal(_fixture.Location, result.JobName.LocationId);
            // Job resource name uses project number for the identifier.
            Assert.Equal(_fixture.ProjectNumber, result.JobName.ProjectId);
            _fixture.jobIds.Add(result.JobName.JobId);
            _fixture.jobTemplateIds.Add(_templateId);

            _fixture.TranscoderChangesPropagated.Eventually(() =>
                 Assert.Equal(_fixture.JobStateSucceeded, _getJobSample.GetJobState(_fixture.ProjectId, _fixture.Location, result.JobName.JobId)
            ));
        }
    }
}
