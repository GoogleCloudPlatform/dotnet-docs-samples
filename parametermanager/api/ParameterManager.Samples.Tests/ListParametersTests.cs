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
public class ListParametersTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly ListParametersSample _sample;

    public ListParametersTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListParametersSample();
    }

    [Fact]
    public void ListParameters()
    {
        Parameter parameter = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Json);
        Parameter parameter1 = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Json);
        IEnumerable<Parameter> result = _sample.ListParameters(projectId: _fixture.ProjectId);
        Assert.NotNull(result);

        _fixture.ParametersToDelete.Add(parameter.ParameterName);
        _fixture.ParametersToDelete.Add(parameter1.ParameterName);
    }
}
