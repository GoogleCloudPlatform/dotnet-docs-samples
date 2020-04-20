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
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

[Collection(nameof(KmsFixture))]
public class DecryptAsymmetricTest
{
    private readonly KmsFixture _fixture;
    private readonly DecryptAsymmetricSample _sample;

    public DecryptAsymmetricTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new DecryptAsymmetricSample();
    }

    [Fact]
    public void DecryptsDataa()
    {
        var plaintext = "testing1234";

        // Get the public key.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(_fixture.ProjectId, _fixture.LocationId, _fixture.KeyRingId, _fixture.AsymmetricDecryptKeyId, "1");
        var publicKey = client.GetPublicKey(keyVersionName);

        // Split the key into blocks and base64-decode the PEM parts.
        var blocks = publicKey.Pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var pem = Convert.FromBase64String(blocks[1]);

        // Create a new RSA key.
        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(pem, out _);

        // Encrypt the data.
        var ciphertext = rsa.Encrypt(Encoding.UTF8.GetBytes(plaintext), RSAEncryptionPadding.OaepSHA256);

        // Run the sample code.
        var result = _sample.DecryptAsymmetric(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.AsymmetricDecryptKeyId, keyVersionId: "1",
            ciphertext: ciphertext);

        Assert.Equal(plaintext, result);
    }
}
