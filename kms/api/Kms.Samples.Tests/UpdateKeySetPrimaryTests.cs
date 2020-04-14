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

using Xunit;

[Collection(nameof(KmsFixture))]
public class UpdateKeySetPrimaryTest
{
    private readonly KmsFixture _fixture;
    private readonly UpdateKeySetPrimarySample _sample;

    public UpdateKeySetPrimaryTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateKeySetPrimarySample();
    }

    [Fact]
    public void SetsPrimary()
    {
        // Create a key.
        var createdKey = _fixture.CreateSymmetricKey(_fixture.RandomId());
        var createdVersion = _fixture.CreateKeyVersion(createdKey.CryptoKeyName.CryptoKeyId);
        var name = createdVersion.CryptoKeyVersionName;

        // Run the sample code.
        var result = _sample.UpdateKeySetPrimary(
            projectId: name.ProjectId, locationId: name.LocationId, keyRingId: name.KeyRingId, keyId: name.CryptoKeyId,
            keyVersionId: name.CryptoKeyVersionId);

        Assert.NotNull(result.Primary);
        Assert.Equal(createdVersion.CryptoKeyVersionName, result.Primary.CryptoKeyVersionName);
    }
}
