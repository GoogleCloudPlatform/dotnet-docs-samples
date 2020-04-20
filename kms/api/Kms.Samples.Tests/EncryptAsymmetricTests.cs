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
public class EncryptAsymmetricTest
{
    private readonly KmsFixture _fixture;
    private readonly EncryptAsymmetricSample _sample;

    public EncryptAsymmetricTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new EncryptAsymmetricSample();
    }

    [Fact]
    public void EncryptsData()
    {
        var message = "testing1234";

        // Run the sample code.
        var ciphertext = _sample.EncryptAsymmetric(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.AsymmetricDecryptKeyId, keyVersionId: "1",
            message: message);

        // Decrypt result.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(_fixture.ProjectId, _fixture.LocationId, _fixture.KeyRingId, _fixture.AsymmetricDecryptKeyId, "1");
        var result = client.AsymmetricDecrypt(keyVersionName, ByteString.CopyFrom(ciphertext));

        Assert.Equal(message, result.Plaintext.ToStringUtf8());
    }
}
