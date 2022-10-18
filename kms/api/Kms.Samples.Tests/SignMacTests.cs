/*
 * Copyright 2021 Google LLC
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
public class SignMacTest
{
    private readonly KmsFixture _fixture;
    private readonly SignMacSample _sample;

    public SignMacTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new SignMacSample();
    }

    [Fact]
    public void EncryptsData()
    {
        var data = "testing1234";

        // Run the sample code.
        var signature = _sample.SignMac(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.MacKeyId, keyVersionId: "1",
            data: data);

        // Verify the signature.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(_fixture.ProjectId, _fixture.LocationId, _fixture.KeyRingId, _fixture.MacKeyId, "1");

        MacVerifyResponse result = client.MacVerify(keyVersionName, ByteString.CopyFromUtf8(data), ByteString.CopyFrom(signature));
        Assert.True(result.Success);
    }
}
