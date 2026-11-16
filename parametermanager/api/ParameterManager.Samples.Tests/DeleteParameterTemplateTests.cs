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
public class DeleteParameterTemplateTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly DeleteParameterTemplateSample _sample;

    public DeleteParameterTemplateTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new DeleteParameterTemplateSample();
    }

    [Fact]
    public void DeleteParameterTemplate()
    {
        Template template = _fixture.CreateTemplate(_fixture.RandomId(), TemplateFormat.Json);

        _sample.DeleteParameterTemplate(projectId: _fixture.ProjectId, templateId: template.TemplateName.TemplateId);

        var exception = Assert.Throws<Grpc.Core.RpcException>(() => _fixture.Client.GetTemplate(template.TemplateName));
        Assert.Equal(Grpc.Core.StatusCode.NotFound, exception.Status.StatusCode);
    }
}
