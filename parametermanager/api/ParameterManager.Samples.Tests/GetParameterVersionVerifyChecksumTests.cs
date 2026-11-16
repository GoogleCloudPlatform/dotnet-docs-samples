/*
 * Copyright 2026 Google LLC
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

using Force.Crc32;
using Google.Cloud.ParameterManager.V1;

[Collection(nameof(ParameterManagerFixture))]
public class GetParameterVersionVerifyChecksumTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly GetParameterVersionVerifyChecksumSample _sample;

    public GetParameterVersionVerifyChecksumTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetParameterVersionVerifyChecksumSample();
    }

    [Fact]
    public void GetParameterVersionVerifyChecksum()
    {
        var (parameterId, versionId, parameter, payload) = _fixture.CreateParameterWithVersion();
        ParameterVersion parameterVersion = _fixture.CreateParameterVersion(parameterId, versionId, payload);

        ParameterVersion result = _sample.GetParameterVersionVerifyChecksum(projectId: _fixture.ProjectId, parameterId: parameterId, versionId: versionId);

        Assert.NotNull(result);
        Assert.Equal(versionId, result.ParameterVersionName.ParameterVersionId);
        Assert.Equal(ParameterVersion.Types.ChecksumSource.ServerGenerated, result.ChecksumSource);
        Assert.Equal(Crc32CAlgorithm.Compute(result.Payload.Data.ToByteArray()), result.Payload.DataCrc32C);
    }
}
