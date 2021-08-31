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
    public class ListJobTemplatesTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobTemplateSample _createSample;
        private readonly ListJobTemplatesSample _listSample;
        private readonly DeleteJobTemplateSample _deleteSample;

        private string _templateId;

        public ListJobTemplatesTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobTemplateSample();
            _listSample = new ListJobTemplatesSample();
            _deleteSample = new DeleteJobTemplateSample();
            _templateId = "my-job-template-" + _fixture.RandomId();

            var result = _fixture.TranscoderChangesPropagated.Eventually(() => _createSample.CreateJobTemplate(
                 projectId: _fixture.ProjectId, location: _fixture.Location,
                 templateId: _templateId));

            Assert.Equal(result.JobTemplateName.LocationId, _fixture.Location);
            // Job template resource name uses project number for the identifier.
            Assert.Equal(result.JobTemplateName.ProjectId, _fixture.ProjectNumber);
            Assert.Equal(result.JobTemplateName.JobTemplateId, _templateId);
            _fixture.jobTemplateIds.Add(_templateId);
        }

        [Fact]
        public void ListsJobTemplates()
        {
            // Run the sample code.
            _fixture.TranscoderChangesPropagated.Eventually(() =>
            {
                var jobTemplates = _listSample.ListJobTemplates(_fixture.ProjectId, location: _fixture.Location);
                string templateName = string.Format("projects/{0}/locations/{1}/jobTemplates/{2}", _fixture.ProjectNumber, _fixture.Location, _templateId);
                Assert.Contains(templateName, jobTemplates);
            });
        }
    }
}
