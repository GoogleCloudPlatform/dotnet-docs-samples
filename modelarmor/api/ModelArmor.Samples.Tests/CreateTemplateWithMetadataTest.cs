/*
 * Copyright 2025 Google LLC
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

using Google.Cloud.ModelArmor.V1;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithMetadataTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithMetadataSample _sample;

        public CreateTemplateWithMetadataTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithMetadataSample();
        }

        [Fact]
        public void CreateTemplateWithMetadata()
        {
            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            TemplateName templateName = _fixture.CreateTemplateName();
            _fixture.RegisterTemplateForCleanup(templateName);
            string templateId = templateName.TemplateId;

            Template createdTemplate = _sample.CreateTemplateWithMetadata(projectId, locationId, templateId);

            Assert.NotNull(createdTemplate.TemplateMetadata);
            Assert.True(createdTemplate.TemplateMetadata.LogTemplateOperations);
            Assert.True(createdTemplate.TemplateMetadata.LogSanitizeOperations);
        }
    }
}
