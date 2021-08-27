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
        private readonly DeleteJobTemplateSample _deleteTemplateSample;
        private readonly CreateJobFromTemplateSample _createJobSample;
        private string _jobId;
        private readonly GetJobStateSample _getJobSample;
        private readonly DeleteJobSample _deleteJobSample;

        public CreateJobFromTemplateTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createTemplateSample = new CreateJobTemplateSample();
            _deleteTemplateSample = new DeleteJobTemplateSample();
            _createJobSample = new CreateJobFromTemplateSample();
            _getJobSample = new GetJobStateSample();
            _deleteJobSample = new DeleteJobSample();
            _templateId = "my-job-template-" + _fixture.RandomId();

            _createTemplateSample.CreateJobTemplate(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                templateId: _templateId);
        }

        [Fact]
        public void CreatesJobFromTemplate()
        {
            string outputUri = "gs://" + _fixture.BucketName + "/test-output-template/";
            // Run the sample code.
            var result = _createJobSample.CreateJobFromTemplate(
                projectId: _fixture.ProjectId, location: _fixture.Location,
                inputUri: _fixture.InputUri, outputUri: outputUri, templateId: _templateId);

            Assert.Equal(result.JobName.LocationId, _fixture.Location);
            // Job resource name uses project number for the identifier.
            Assert.Equal(result.JobName.ProjectId, _fixture.ProjectNumber);
            _jobId = result.JobName.JobId;
            _fixture.jobIds.Add(_jobId);
            _fixture.jobTemplateIds.Add(_templateId);

            Job.Types.ProcessingState state = Job.Types.ProcessingState.Unspecified;

            for (int attempt = 0; attempt < 5; attempt++)
            {
                Thread.Sleep(60000);
                try
                {
                    state = _getJobSample.GetJobState(projectId: _fixture.ProjectId, location: _fixture.Location, _jobId);
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
