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
    public class GetJobTemplateTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobTemplateSample _createSample;
        private readonly GetJobTemplateSample _getSample;

        private string _templateId;

        public GetJobTemplateTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobTemplateSample();
            _getSample = new GetJobTemplateSample();
            _templateId = $"my-job-template-{_fixture.RandomId()}";

            _createSample.CreateJobTemplate(
                _fixture.ProjectId, _fixture.Location,
                _templateId);
            _fixture.JobTemplateIds.Add(_templateId);
        }

        [Fact]
        public void GetsJobTemplate()
        {
            // Run the sample code.
            var result = _getSample.GetJobTemplate(
                _fixture.ProjectId, _fixture.Location,
                _templateId);

            Assert.Equal(_fixture.Location, result.JobTemplateName.LocationId);
            // Job template resource name uses project number for the identifier.
            Assert.Equal(_fixture.ProjectNumber, result.JobTemplateName.ProjectId);
            Assert.Equal(_templateId, result.JobTemplateName.JobTemplateId);
        }
    }
}
