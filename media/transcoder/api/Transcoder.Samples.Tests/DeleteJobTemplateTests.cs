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
    public class DeleteJobTemplateTest
    {
        private TranscoderFixture _fixture;
        private readonly CreateJobTemplateSample _createSample;
        private readonly DeleteJobTemplateSample _deleteSample;

        private string _templateId;

        public DeleteJobTemplateTest(TranscoderFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateJobTemplateSample();
            _deleteSample = new DeleteJobTemplateSample();
            _templateId = $"my-job-template-{_fixture.RandomId()}";

            _createSample.CreateJobTemplate(
                _fixture.ProjectId, _fixture.Location,
                _templateId);
            _fixture.JobTemplateIds.Add(_templateId);
        }

        [Fact]
        public void DeletesJobTemplate() =>
            // Run the sample code.
            _deleteSample.DeleteJobTemplate(_fixture.ProjectId, _fixture.Location, _templateId);
    }
}
