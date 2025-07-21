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

using System;
using System.Collections.Generic;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithLabelsTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithLabelsSample _sample;

        public CreateTemplateWithLabelsTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithLabelsSample();
        }

        [Fact]
        public void CreateTemplateWithLabels()
        {
            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            TemplateName templateName = _fixture.CreateTemplateName();
            _fixture.RegisterTemplateForCleanup(templateName);
            string templateId = templateName.TemplateId;

            // Run the sample.
            Template createdTemplate = _sample.CreateTemplateWithLabels(
                projectId: projectId,
                locationId: locationId,
                templateId: templateId
            );

            // Verify the template was created successfully.
            Assert.NotNull(createdTemplate);
            Assert.Contains(templateId, createdTemplate.Name);

            // Since labels is not a part of the Create Template API response,
            // we retrieve the template to verify labels.
            ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
            };
            ModelArmorClient client = clientBuilder.Build();
            Template retrievedTemplate = client.GetTemplate(createdTemplate.Name);

            // Verify the labels.
            Assert.NotNull(retrievedTemplate.Labels);
            Assert.Equal(2, retrievedTemplate.Labels.Count);
            Assert.Equal("value1", retrievedTemplate.Labels["key1"]);
            Assert.Equal("value2", retrievedTemplate.Labels["key2"]);
        }
    }
}
