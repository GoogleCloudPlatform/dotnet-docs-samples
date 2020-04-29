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
using Xunit;

[Collection(nameof(KmsFixture))]
public class CreateKeyRingTest : IDisposable
{
    private readonly KmsFixture _fixture;
    private readonly CreateKeyRingSample _sample;
    private readonly string _keyRingId;

    public CreateKeyRingTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _keyRingId = _fixture.RandomId();
        _sample = new CreateKeyRingSample();
    }

    public void Dispose()
    {
        _fixture.DisposeKeyRing(_keyRingId);
    }

    [Fact]
    public void CreatesKeyRing()
    {
        // Run the sample code.
        var result = _sample.CreateKeyRing(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId,
            id: _keyRingId);

        // Get the key ring.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        var keyRing = client.GetKeyRing(result.KeyRingName);

        Assert.Contains(_keyRingId, keyRing.Name);
    }
}
