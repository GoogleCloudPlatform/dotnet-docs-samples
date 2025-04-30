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
public class CreateStructuredParameterTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateStructuredParameterSample _sample;

    public CreateStructuredParameterTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateStructuredParameterSample();
    }

    [Fact]
    public void CreateStructuredParameter()
    {
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerFixture.LocationId, _fixture.RandomId());
        Parameter result = _sample.CreateStructuredParameter(
            projectId: parameterName.ProjectId, parameterId: parameterName.ParameterId, format: ParameterFormat.Json);
        _fixture.ParametersToDelete.Add(parameterName);

        Assert.NotNull(result);
        Assert.Equal(ParameterFormat.Json, result.Format);
        Assert.Equal(parameterName.ParameterId, result.ParameterName.ParameterId);
    }
}
