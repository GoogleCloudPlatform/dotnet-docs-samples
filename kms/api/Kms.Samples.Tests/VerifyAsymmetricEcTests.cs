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
public class VerifyAsymmetricSignEc
{
    private readonly KmsFixture _fixture;
    private readonly VerifyAsymmetricSignatureEcSample _sample;

    public VerifyAsymmetricSignEc(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new VerifyAsymmetricSignatureEcSample();
    }

    [Fact(Skip = ".NET lacks support for EC keys")]
    public void VerifiesData()
    {
        Assert.True(false);
    }
}
