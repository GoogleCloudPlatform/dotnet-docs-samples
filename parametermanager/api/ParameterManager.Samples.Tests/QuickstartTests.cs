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
public class QuickstartTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly QuickstartSample _sample;

    public QuickstartTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new QuickstartSample();
    }

    [Fact]
    public void Quickstart()
    {
        string parameterId = _fixture.RandomId();
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerFixture.LocationId, parameterId);
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, parameterId, _fixture.RandomId());
        _sample.Quickstart(
            projectId: parameterVersionName.ProjectId, parameterId: parameterVersionName.ParameterId, versionId: parameterVersionName.ParameterVersionId);
        _fixture.ParametersToDelete.Add(parameterName);
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

        ParameterManagerClient client = ParameterManagerClient.Create();
        ParameterVersion result = client.GetParameterVersion(parameterVersionName);

        Assert.NotNull(result);
        Assert.Equal(parameterVersionName.ParameterVersionId, result.ParameterVersionName.ParameterVersionId);
    }
}
