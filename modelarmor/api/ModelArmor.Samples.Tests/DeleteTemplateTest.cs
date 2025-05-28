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
using Google.Cloud.ModelArmor.V1;
using Grpc.Core;
using Xunit;

namespace ModelArmor.Samples.Tests
{
    public class DeleteTemplateTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateSample _create_template_sample;
        private readonly DeleteTemplateSample _delete_template_sample;

        public DeleteTemplateTests(ModelArmorFixture fixture)
        {
            _fixture = fixture;
            _create_template_sample = new CreateTemplateSample();
            _delete_template_sample = new DeleteTemplateSample();
        }

        [Fact]
        public void DeleteTemplateTest()
        {
            // Create a template for testing purpose.
            TemplateName templateName = _fixture.CreateTemplateName();
            Template createdTemplate = _create_template_sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId
            );

            // Delete the template using the sample.
            _delete_template_sample.DeleteTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId
            );

            // Try to get the template directly - should throw a "not found" exception.
            var exception = Assert.Throws<RpcException>(() =>
            {
                var getTemplateSample = new GetTemplateSample();
                getTemplateSample.GetTemplate(
                    projectId: _fixture.ProjectId,
                    locationId: _fixture.LocationId,
                    templateId: templateName.TemplateId
                );
            });

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public void DeleteTemplateTest_NonExistentTemplate()
        {
            // Generate a random template ID that doesn't exist.
            string nonExistentTemplateId =
                $"non-existent-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            // Attempt to delete a non-existent template should throw an exception.
            var exception = Assert.Throws<RpcException>(() =>
                _delete_template_sample.DeleteTemplate(
                    projectId: _fixture.ProjectId,
                    locationId: _fixture.LocationId,
                    templateId: nonExistentTemplateId
                )
            );

            // Verify the exception contains the expected error code.
            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }
    }
}
