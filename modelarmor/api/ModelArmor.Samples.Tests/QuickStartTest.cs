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
using Xunit;

[Collection(nameof(ModelArmorFixture))]
public class QuickstartTests
{
    private readonly ModelArmorFixture _fixture;
    private readonly QuickstartSample _sample;

    public QuickstartTests(ModelArmorFixture fixture)
    {
        _fixture = fixture;
        _sample = new QuickstartSample();
    }

    [Fact]
    public void Runs()
    {
        // Get template name from fixture
        TemplateName templateName = _fixture.CreateTemplateName();
        _fixture.RegisterTemplateForCleanup(templateName);

        // Run the quickstart sample
        _sample.Quickstart(
            projectId: templateName.ProjectId,
            locationId: templateName.LocationId,
            templateId: templateName.TemplateId
        );
    }
}
