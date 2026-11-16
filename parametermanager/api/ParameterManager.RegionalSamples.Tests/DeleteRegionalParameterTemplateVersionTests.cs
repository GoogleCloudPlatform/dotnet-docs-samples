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

[Collection(nameof(ParameterManagerRegionalFixture))]
public class DeleteRegionalParameterTemplateVersionTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly DeleteRegionalParameterTemplateVersionSample _sample;

    public DeleteRegionalParameterTemplateVersionTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteRegionalParameterTemplateVersionSample();
    }

    [Fact]
    public void DeleteRegionalParameterTemplateVersion()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);
        TemplateVersion version = _fixture.CreateTemplateVersion(template.TemplateName.TemplateId, _fixture.RandomId(), "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}");

        _sample.DeleteRegionalParameterTemplateVersion(projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, templateId: template.TemplateName.TemplateId, versionId: version.TemplateVersionName.TemplateVersionId);

        var exception = Assert.Throws<Grpc.Core.RpcException>(() => _fixture.Client.GetTemplateVersion(version.TemplateVersionName));
        Assert.Equal(Grpc.Core.StatusCode.NotFound, exception.Status.StatusCode);
    }
}
