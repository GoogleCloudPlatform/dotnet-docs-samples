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
using Google.Protobuf;
using Xunit;

[Collection(nameof(KmsFixture))]
public class DecryptSymmetricTest
{
    private readonly KmsFixture _fixture;
    private readonly DecryptSymmetricSample _sample;

    public DecryptSymmetricTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new DecryptSymmetricSample();
    }

    [Fact]
    public void DecryptsData()
    {
        var plaintext = "testing1234";

        // Create some ciphertext.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        CryptoKeyName keyName = new CryptoKeyName(_fixture.ProjectId, _fixture.LocationId, _fixture.KeyRingId, _fixture.SymmetricKeyId);
        var result = client.Encrypt(keyName, ByteString.CopyFromUtf8(plaintext));

        // Run the sample code.
        var response = _sample.DecryptSymmetric(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.SymmetricKeyId,
            ciphertext: result.Ciphertext.ToByteArray());

        Assert.Equal(plaintext, response);
    }
}
