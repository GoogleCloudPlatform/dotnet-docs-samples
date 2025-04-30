/*
 * Copyright 2025 Google LLC
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
public class CreateParameterVersionWithSecretTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParameterVersionWithSecretSample _sample;

    public CreateParameterVersionWithSecretTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParameterVersionWithSecretSample();
    }

    [Fact]
    public void CreateParameterVersionWithSecret()
    {
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, _fixture.RandomId(), _fixture.RandomId());
        Parameter parameter = _fixture.CreateParameter(parameterVersionName.ParameterId, ParameterFormat.Json);
        string secretId = "projects/project-id/secrets/secret-id/versions/latest";
        ParameterVersion result = _sample.CreateParameterVersionWithSecret(
            projectId: parameterVersionName.ProjectId, parameterId: parameterVersionName.ParameterId, versionId: parameterVersionName.ParameterVersionId, secretId: secretId);
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

        Assert.NotNull(result);
        Assert.Equal(parameterVersionName.ParameterVersionId, result.ParameterVersionName.ParameterVersionId);
    }
}
