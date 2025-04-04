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

using Google.Cloud.Kms.V1;
using Google.Cloud.ParameterManager.V1;

[Collection(nameof(ParameterManagerRegionalFixture))]
public class RemoveRegionalParameterKmsKeyTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly RemoveRegionalParameterKmsKeySample _sample;

    public RemoveRegionalParameterKmsKeyTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new RemoveRegionalParameterKmsKeySample();
    }

    [Fact]
    public void RemoveRegionalParameterKmsKey()
    {
        ParameterName parameterName = _fixture.ParameterName;
        Parameter result = _sample.RemoveRegionalParameterKmsKey(
          projectId: parameterName.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, parameterId: parameterName.ParameterId);

        Assert.NotNull(result);
        Assert.Equal(result.ParameterName.ParameterId, parameterName.ParameterId);
    }
}
