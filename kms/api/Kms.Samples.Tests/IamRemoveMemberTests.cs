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
public class IamRemoveMemberTest
{
    private readonly KmsFixture _fixture;
    private readonly IamRemoveMemberSample _sample;

    public IamRemoveMemberTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new IamRemoveMemberSample();
    }

    [Fact]
    public void RemovesMembers()
    {
        // Run the sample code.
        var result = _sample.IamRemoveMember(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId, keyId: _fixture.AsymmetricSignEcKeyId,
            member: "group:test@google.com");

        Assert.DoesNotContain(result.Bindings,
            binding => binding.Role == "roles/cloudkms.cryptoKeyEncrypterDecrypter" && binding.Members.Contains("group:test@google.com"));
    }
}
