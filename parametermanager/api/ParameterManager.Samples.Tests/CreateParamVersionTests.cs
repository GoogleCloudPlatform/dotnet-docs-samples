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
public class CreateParamVersionTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParamVersionSample _sample;

    public CreateParamVersionTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParamVersionSample();
    }

    [Fact]
    public void CreateParamVersion()
    {
        ParameterVersionName parameterVersionName = _fixture.ParameterVersionName;
        string payload = _fixture.Payload;
        ParameterVersion result = _sample.CreateParamVersion(
          projectId: parameterVersionName.ProjectId, parameterId: parameterVersionName.ParameterId, versionId: parameterVersionName.ParameterVersionId, payload: payload);

        Assert.NotNull(result);
        Assert.Equal(result.Name, parameterVersionName.ToString());
    }
}
