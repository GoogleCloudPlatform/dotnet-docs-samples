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
public class ListParameterVersionsTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly ListParameterVersionsSample _sample;

    public ListParameterVersionsTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListParameterVersionsSample();
    }

    [Fact]
    public void ListParameterVersions()
    {
        string parameterId = _fixture.RandomId();
        Parameter parameter = _fixture.CreateParameter(parameterId, ParameterFormat.Unformatted);
        string payload = "test123";
        ParameterVersion parameterVersion = _fixture.CreateParameterVersion(parameterId, _fixture.RandomId(), payload);
        ParameterVersion parameterVersion1 = _fixture.CreateParameterVersion(parameterId, _fixture.RandomId(), payload);

        IEnumerable<ParameterVersion> result = _sample.ListParameterVersions(projectId: _fixture.ProjectId, parameterId: parameterId);
        Assert.NotNull(result);
    }
}
