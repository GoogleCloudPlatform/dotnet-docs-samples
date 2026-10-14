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
using System.Text.Json;

[Collection(nameof(ParameterManagerFixture))]
public class RenderParameterTemplateVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly RenderParameterTemplateVersionSample _sample;

    public RenderParameterTemplateVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new RenderParameterTemplateVersionSample();
    }

    [Fact]
    public void RenderParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersion templateVersion = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");
        Parameter parameter = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Json);
        ParameterVersion parameterVersion = _fixture.CreateParameterVersion(parameter.ParameterName.ParameterId, _fixture.RandomId(), "{\"username\": \"test-user\", \"host\": \"localhost\"}");

        RenderTemplateVersionResponse result = _sample.RenderParameterTemplateVersion(projectId: _fixture.ProjectId, templateId: template.TemplateName.TemplateId, versionId: templateVersion.TemplateVersionName.TemplateVersionId, parameterId: parameter.ParameterName.ParameterId, parameterVersionId: parameterVersion.ParameterVersionName.ParameterVersionId);

        Assert.NotNull(result);
        Assert.Contains("{{.username}}", Encoding.UTF8.GetString(result.Payload.Data.ToByteArray()));
        using JsonDocument rendered = JsonDocument.Parse(result.RenderedPayload.ToByteArray());
        Assert.Equal("test-user", rendered.RootElement.GetProperty("username").GetString());
        Assert.Equal("localhost", rendered.RootElement.GetProperty("host").GetString());
    }
}
