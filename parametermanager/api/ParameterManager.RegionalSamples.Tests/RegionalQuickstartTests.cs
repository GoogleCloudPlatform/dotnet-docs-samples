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

[Collection(nameof(ParameterManagerRegionalFixture))]
public class RegionalQuickstartTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly RegionalQuickstartSample _sample;

    public RegionalQuickstartTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new RegionalQuickstartSample();
    }

    [Fact]
    public void RegionalQuickstart()
    {
        string parameterId = _fixture.RandomId();
        ParameterName parameterName = new ParameterName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, parameterId);
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, parameterId, _fixture.RandomId());

        _sample.RegionalQuickstart(
            projectId: parameterVersionName.ProjectId, parameterId: parameterVersionName.ParameterId, locationId: ParameterManagerRegionalFixture.LocationId, versionId: parameterVersionName.ParameterVersionId);
        _fixture.ParametersToDelete.Add(parameterName);
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

        // Create the client with the regional endpoint
        ParameterManagerClient client = _fixture.Client;
        ParameterVersion result = client.GetParameterVersion(parameterVersionName);

        Assert.NotNull(result);
        Assert.Equal(parameterVersionName.ParameterVersionId, result.ParameterVersionName.ParameterVersionId);
    }
}
