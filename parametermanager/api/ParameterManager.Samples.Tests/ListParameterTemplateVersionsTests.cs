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
public class ListParameterTemplateVersionsTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly ListParameterTemplateVersionsSample _sample;

    public ListParameterTemplateVersionsTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListParameterTemplateVersionsSample();
    }

    [Fact]
    public void ListParameterTemplateVersions()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersion version = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");
        TemplateVersion version1 = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");

        IEnumerable<TemplateVersion> result = _sample.ListParameterTemplateVersions(projectId: _fixture.ProjectId, templateId: template.TemplateName.TemplateId);

        Assert.NotNull(result);
        List<string> names = result.Select(v => v.Name).ToList();
        Assert.Contains(version.Name, names);
        Assert.Contains(version1.Name, names);
    }
}
