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
using System.Security.Cryptography;
using System.Text;
using Xunit;

[Collection(nameof(KmsFixture))]
public class VerifyAsymmetricSignRsa
{
    private readonly KmsFixture _fixture;
    private readonly VerifyAsymmetricSignatureRsaSample _sample;

    public VerifyAsymmetricSignRsa(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new VerifyAsymmetricSignatureRsaSample();
    }

    [Fact]
    public void VerifiesData()
    {
        var message = "testing1234";

        // Calculate the message digest.
        var sha256 = SHA256.Create();
        var digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Sign the data
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(_fixture.ProjectId, _fixture.LocationId, _fixture.KeyRingId, _fixture.AsymmetricSignRsaKeyId, "1");
        var result = client.AsymmetricSign(keyVersionName, new Digest
        {
            Sha256 = ByteString.CopyFrom(digest),
        });

        // Run the sample.
        var verified = _sample.VerifyAsymmetricSignatureRsa(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.AsymmetricSignRsaKeyId, keyVersionId: "1",
            message: message,
            signature: result.Signature.ToByteArray());

        Assert.True(verified);
    }
}
