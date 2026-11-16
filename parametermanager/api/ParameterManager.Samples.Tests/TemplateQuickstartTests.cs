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
using System.Text.Json;

[Collection(nameof(ParameterManagerFixture))]
public class TemplateQuickstartTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly TemplateQuickstartSample _sample;

    public TemplateQuickstartTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new TemplateQuickstartSample();
    }

    [Fact]
    public void TemplateQuickstart()
    {
        string templateId = _fixture.RandomId();
        string parameterId = _fixture.RandomId();
        string versionId = _fixture.RandomId();
        TemplateVersionName templateVersionName = new TemplateVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, templateId, versionId);
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, parameterId, versionId);

        _fixture.TemplatesToDelete.Add(new TemplateName(_fixture.ProjectId, ParameterManagerFixture.LocationId, templateId));
        _fixture.TemplateVersionsToDelete.Add(templateVersionName);
        _fixture.ParametersToDelete.Add(new ParameterName(_fixture.ProjectId, ParameterManagerFixture.LocationId, parameterId));
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

        _sample.TemplateQuickstart(projectId: _fixture.ProjectId, templateId: templateId, parameterId: parameterId, versionId: versionId);

        RenderTemplateVersionResponse result = _fixture.Client.RenderTemplateVersion(new RenderTemplateVersionRequest
        {
            TemplateVersionName = templateVersionName,
            ParameterVersionAsParameterVersionName = parameterVersionName
        });
        using JsonDocument rendered = JsonDocument.Parse(result.RenderedPayload.ToByteArray());
        Assert.Equal("test-user", rendered.RootElement.GetProperty("username").GetString());
        Assert.Equal("localhost", rendered.RootElement.GetProperty("host").GetString());
    }
}
