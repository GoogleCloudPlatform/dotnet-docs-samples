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
public class CreateStructuredParamTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateStructuredParamSample _sample;

    public CreateStructuredParamTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateStructuredParamSample();
    }

    [Fact]
    public void CreateStructuredParam()
    {
        ParameterName parameterName = _fixture.ParameterNameWithFormat;
        Parameter result = _sample.CreateStructuredParam(
          projectId: parameterName.ProjectId, parameterId: parameterName.ParameterId, format: ParameterFormat.Json);

        Assert.NotNull(result);
        Assert.Equal(result.Format.ToString(), ParameterFormat.Json.ToString());
        Assert.Equal(result.Name, parameterName.ToString());
    }
}
