/*
 * Copyright 2026 Google LLC
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

using Google.Cloud.ParameterManager.V1;

[Collection(nameof(ParameterManagerFixture))]
public class CreateParameterTemplateTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParameterTemplateSample _sample;

    public CreateParameterTemplateTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParameterTemplateSample();
    }

    [Fact]
    public void CreateParameterTemplate()
    {
        string templateId = _fixture.RandomId();
        TemplateName templateName = new TemplateName(_fixture.ProjectId, ParameterManagerFixture.LocationId, templateId);

        Template result = _sample.CreateParameterTemplate(projectId: _fixture.ProjectId, templateId: templateId, format: TemplateFormat.Json);
        _fixture.TemplatesToDelete.Add(templateName);

        Assert.NotNull(result);
        Assert.Equal(templateId, result.TemplateName.TemplateId);
        Assert.Equal(TemplateFormat.Json, result.Format);
    }
}
