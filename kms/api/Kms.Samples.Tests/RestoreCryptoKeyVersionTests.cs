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
public class RestoreKeyVersionTest
{
    private readonly KmsFixture _fixture;
    private readonly RestoreKeyVersionSample _sample;

    public RestoreKeyVersionTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new RestoreKeyVersionSample();
    }

    [Fact]
    public void RestoresKeyVersions()
    {
        // Create a key version.
        var key = _fixture.CreateSymmetricKey(_fixture.RandomId());
        var keyVersion = _fixture.CreateKeyVersion(key.CryptoKeyName.CryptoKeyId);

        // Destroy the key.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        client.DestroyCryptoKeyVersion(keyVersion.CryptoKeyVersionName);

        // Run the sample code.
        var name = keyVersion.CryptoKeyVersionName;
        var response = _sample.RestoreKeyVersion(
            projectId: name.ProjectId, locationId: name.LocationId, keyRingId: name.KeyRingId, keyId: name.CryptoKeyId, keyVersionId: name.CryptoKeyVersionId);

        // Verify disabled (restored keys come back disabled).
        Assert.Equal(CryptoKeyVersion.Types.CryptoKeyVersionState.Disabled, response.State);
    }
}
