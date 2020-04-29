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
using System;

[Collection(nameof(KmsFixture))]
public class GetKeyVersionAttestationTests
{
    private readonly KmsFixture _fixture;
    private readonly GetKeyVersionAttestationSample _sample;

    public GetKeyVersionAttestationTests(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new GetKeyVersionAttestationSample();
    }

    [Fact]
    public void ErrorsOnNonHsm()
    {
        // Run the sample code with a non-HSM key
        Assert.Throws<InvalidOperationException>(() => _sample.GetKeyVersionAttestation(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.SymmetricKeyId, keyVersionId: "1"));
    }

    [Fact]
    public void GetsAttestations()
    {
        // Run the sample code.
        var result = _sample.GetKeyVersionAttestation(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.HsmKeyId, keyVersionId: "1");
        Assert.NotEmpty(result);
    }
}
