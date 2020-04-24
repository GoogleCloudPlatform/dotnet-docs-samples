/*
 * Copyright 2020 Google LLC
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
using Xunit;

[Collection(nameof(KmsFixture))]
public class CreateKeyVersionTest
{
    private readonly KmsFixture _fixture;
    private readonly CreateKeyVersionSample _sample;

    public CreateKeyVersionTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateKeyVersionSample();
    }

    [Fact]
    public void CreatesKeyVersion()
    {
        // Run the sample code.
        var result = _sample.CreateKeyVersion(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.SymmetricKeyId);

        // Get the key version.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        var response = client.GetCryptoKeyVersion(result.CryptoKeyVersionName);

        Assert.NotNull(response.CryptoKeyVersionName.CryptoKeyVersionId);
    }
}
