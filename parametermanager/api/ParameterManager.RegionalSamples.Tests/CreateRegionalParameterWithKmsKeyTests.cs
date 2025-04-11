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
public class CreateRegionalParameterWithKmsKeyTests
{
    private readonly ParameterManagerRegionalFixture _fixture;
    private readonly CreateRegionalParameterWithKmsKeySample _sample;

    public CreateRegionalParameterWithKmsKeyTests(ParameterManagerRegionalFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateRegionalParameterWithKmsKeySample();
    }

    [Fact]
    public void CreateRegionalParameterWithKmsKey()
    {
        string KeyId = _fixture.RandomId();
        CryptoKey cryptoKey = _fixture.CreateHsmKey(_fixture.ProjectId, KeyId, "csharp-test-key-ring");
        CryptoKeyVersionName cryptoKeyVersionName = new CryptoKeyVersionName(_fixture.ProjectId, ParameterManagerRegionalFixture.LocationId, "csharp-test-key-ring", KeyId, "1");

        string parameterId = _fixture.RandomId();
        Parameter result = _sample.CreateRegionalParameterWithKmsKey(
          projectId: _fixture.ProjectId, locationId: ParameterManagerRegionalFixture.LocationId, parameterId: parameterId, kmsKey: cryptoKey.Name);

        Assert.NotNull(result);
        Assert.Equal(result.ParameterName.ParameterId, parameterId);
        Assert.Equal(result.KmsKey, cryptoKey.Name);

        _fixture.ParametersToDelete.Add(result.ParameterName);
        _fixture.CryptoKeyVersionsToDelete.Add(cryptoKeyVersionName);
    }
}
