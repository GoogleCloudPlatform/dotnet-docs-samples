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
using Google.Protobuf;
using System.Text;

[Collection(nameof(ParameterManagerRegionalFixture))]
public class UpdateRegionalParameterKmsKeyTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly UpdateRegionalParameterKmsKeySample _sample;

    public UpdateRegionalParameterKmsKeyTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateRegionalParameterKmsKeySample();
        _fixture.GetKeyRing(_fixture.ProjectId, ParameterManagerRegionalFixture.KeyRingId);
    }

    [Fact]
    public void UpdateRegionalParameterKmsKey()
    {
        string KeyId = _fixture.RandomId();
        string KeyId1 = _fixture.RandomId();
        CryptoKey cryptoKey = _fixture.CreateHsmKey(_fixture.ProjectId, KeyId, ParameterManagerRegionalFixture.KeyRingId);
        CryptoKey cryptoKey1 = _fixture.CreateHsmKey(_fixture.ProjectId, KeyId1, ParameterManagerRegionalFixture.KeyRingId);

        string parameterId = _fixture.RandomId();
        _fixture.CreateParameterWithKmsKey(parameterId, ParameterFormat.Unformatted, cryptoKey.Name);

        Parameter result = _sample.UpdateRegionalParameterKmsKey(
          projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, parameterId: parameterId, kmsKey: cryptoKey1.Name);

        Assert.NotNull(result);
        Assert.Equal(parameterId, result.ParameterName.ParameterId);
        Assert.Equal(cryptoKey1.Name, result.KmsKey);
    }
}
