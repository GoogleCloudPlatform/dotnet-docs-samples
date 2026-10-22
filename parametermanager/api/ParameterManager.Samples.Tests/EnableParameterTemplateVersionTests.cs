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
using Google.Protobuf.WellKnownTypes;

[Collection(nameof(ParameterManagerFixture))]
public class EnableParameterTemplateVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly EnableParameterTemplateVersionSample _sample;

    public EnableParameterTemplateVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new EnableParameterTemplateVersionSample();
    }

    [Fact]
    public void EnableParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersion version = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");
        _fixture.Client.UpdateTemplateVersion(new UpdateTemplateVersionRequest
        {
            TemplateVersion = new TemplateVersion { Name = version.Name, Disabled = true },
            UpdateMask = new FieldMask { Paths = { "disabled" } }
        });

        TemplateVersion result = _sample.EnableParameterTemplateVersion(projectId: _fixture.ProjectId, templateId: template.TemplateName.TemplateId, versionId: version.TemplateVersionName.TemplateVersionId);

        Assert.NotNull(result);
        Assert.Equal(version.Name, result.Name);
        Assert.False(result.Disabled);
        Assert.False(_fixture.Client.GetTemplateVersion(version.TemplateVersionName).Disabled);
    }
}
