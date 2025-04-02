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

[Collection(nameof(ParameterManagerFixture))]
public class RemoveParamKmsKeyTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly RemoveParamKmsKeySample _sample;

    public RemoveParamKmsKeyTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new RemoveParamKmsKeySample();
    }

    [Fact]
    public void RemoveParamKmsKey()
    {
        ParameterName parameterName = _fixture.ParameterName;
        Parameter result = _sample.RemoveParamKmsKey(
          projectId: parameterName.ProjectId, parameterId: parameterName.ParameterId);

        Assert.NotNull(result);
        Assert.Equal(result.Name, parameterName.ToString());
    }
}
