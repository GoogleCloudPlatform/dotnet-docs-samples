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

[Collection(nameof(ParameterManagerRegionalFixture))]
public class CreateRegionalParameterTemplateVersionTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly CreateRegionalParameterTemplateVersionSample _sample;

    public CreateRegionalParameterTemplateVersionTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalParameterTemplateVersionSample();
    }

    [Fact]
    public void CreateRegionalParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersionName templateVersionName = new TemplateVersionName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, template.TemplateName.TemplateId, _fixture.RandomId());

        TemplateVersion result = _sample.CreateRegionalParameterTemplateVersion(projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, templateId: templateVersionName.TemplateId, versionId: templateVersionName.TemplateVersionId);
        _fixture.TemplateVersionsToDelete.Add(templateVersionName);

        Assert.NotNull(result);
        Assert.Equal(templateVersionName.TemplateVersionId, result.TemplateVersionName.TemplateVersionId);
        Assert.Contains("{{.username}}", Encoding.UTF8.GetString(_fixture.Client.GetTemplateVersion(templateVersionName).Payload.Data.ToByteArray()));
    }
}
