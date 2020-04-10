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

using Google.Cloud.Kms.V1;

[Collection(nameof(KmsFixture))]
public class CreateKeyRotationScheduleTest
{
    private readonly KmsFixture _fixture;
    private readonly CreateKeyRotationScheduleSample _sample;

    public CreateKeyRotationScheduleTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateKeyRotationScheduleSample();
    }

    [Fact]
    public void CreatesKey()
    {
        // Run the sample code.
        var result = _sample.CreateKeyRotationSchedule(
          projectId: _fixture.ProjectId, locationId: _fixture.LocationId, keyRingId: _fixture.KeyRingId,
          id: _fixture.RandomId());

        // Get the key.
        var client = KeyManagementServiceClient.Create();
        var key = client.GetCryptoKey(new GetCryptoKeyRequest
        {
            CryptoKeyName = result.CryptoKeyName,
        });

        Assert.Equal(2_592_000, key.RotationPeriod.Seconds);
        Assert.NotNull(key.NextRotationTime);
    }
}
