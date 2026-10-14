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

[Collection(nameof(ParameterManagerRegionalFixture))]
public class RegionalTemplateQuickstartTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly RegionalTemplateQuickstartSample _sample;

    public RegionalTemplateQuickstartTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new RegionalTemplateQuickstartSample();
    }

    [Fact]
    public void RegionalTemplateQuickstart()
    {
        string templateId = _fixture.RandomId();
        string parameterId = _fixture.RandomId();
        string versionId = _fixture.RandomId();
        TemplateVersionName templateVersionName = new TemplateVersionName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, templateId, versionId);
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, parameterId, versionId);

        _sample.RegionalTemplateQuickstart(projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, templateId: templateId, parameterId: parameterId, versionId: versionId);
        _fixture.TemplatesToDelete.Add(new TemplateName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, templateId));
        _fixture.TemplateVersionsToDelete.Add(templateVersionName);
        _fixture.ParametersToDelete.Add(new ParameterName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, parameterId));
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

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
