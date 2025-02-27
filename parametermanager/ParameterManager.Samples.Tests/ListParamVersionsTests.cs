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
public class ListParamVersionsTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly ListParamVersionsSample _sample;

    public ListParamVersionsTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new ListParamVersionsSample();
    }

    [Fact]
    public void ListParamVersions()
    {
        ParameterVersionName parameterVersionName = _fixture.ParameterVersionName;
        IEnumerable<ParameterVersion> result = _sample.ListParamVersions(projectId: _fixture.ProjectId, parameterId: parameterVersionName.ParameterId);
        Assert.NotNull(result);
    }
}
