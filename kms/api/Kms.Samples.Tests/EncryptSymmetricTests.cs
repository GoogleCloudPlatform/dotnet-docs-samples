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
public class EncryptSymmetricTest
{
    private readonly KmsFixture _fixture;
    private readonly EncryptSymmetricSample _sample;

    public EncryptSymmetricTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new EncryptSymmetricSample();
    }

    [Fact]
    public void EncryptsData()
    {
        var message = "testing1234";

        // Run the sample code.
        var result = _sample.EncryptSymmetric(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.SymmetricKeyId,
            message: message);

        // Attempt to decrypt to verify success.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        var response = client.Decrypt(_fixture.SymmetricKeyName, ByteString.CopyFrom(result));

        Assert.Equal(message, response.Plaintext.ToStringUtf8());
    }
}
