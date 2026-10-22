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
using System.Text;

[Collection(nameof(ParameterManagerFixture))]
public class CreateParameterTemplateVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParameterTemplateVersionSample _sample;

    public CreateParameterTemplateVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParameterTemplateVersionSample();
    }

    [Fact]
    public void CreateParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersionName templateVersionName = new TemplateVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, template.TemplateName.TemplateId, _fixture.RandomId());

        TemplateVersion result = _sample.CreateParameterTemplateVersion(projectId: _fixture.ProjectId, templateId: templateVersionName.TemplateId, versionId: templateVersionName.TemplateVersionId);
        _fixture.TemplateVersionsToDelete.Add(templateVersionName);

        Assert.NotNull(result);
        Assert.Equal(templateVersionName.TemplateVersionId, result.TemplateVersionName.TemplateVersionId);
        Assert.Contains("{{.username}}", Encoding.UTF8.GetString(_fixture.Client.GetTemplateVersion(templateVersionName).Payload.Data.ToByteArray()));
    }
}
