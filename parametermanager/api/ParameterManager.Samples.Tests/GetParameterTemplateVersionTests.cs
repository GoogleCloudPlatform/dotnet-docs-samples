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
public class GetParameterTemplateVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly GetParameterTemplateVersionSample _sample;

    public GetParameterTemplateVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetParameterTemplateVersionSample();
    }

    [Fact]
    public void GetParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersion version = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");

        TemplateVersion result = _sample.GetParameterTemplateVersion(projectId: _fixture.ProjectId, templateId: template.TemplateName.TemplateId, versionId: version.TemplateVersionName.TemplateVersionId);

        Assert.NotNull(result);
        Assert.Equal(version.Name, result.Name);
        Assert.Contains("{{.username}}", Encoding.UTF8.GetString(result.Payload.Data.ToByteArray()));
    }
}
