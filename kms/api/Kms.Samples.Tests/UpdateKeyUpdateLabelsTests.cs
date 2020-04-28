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
public class UpdateKeyUpdateLabelsTest
{
    private readonly KmsFixture _fixture;
    private readonly UpdateKeyUpdateLabelsSample _sample;

    public UpdateKeyUpdateLabelsTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new UpdateKeyUpdateLabelsSample();
    }

    [Fact]
    public void UpdatesLabels()
    {
        // Create a key.
        var createdKey = _fixture.CreateSymmetricKey(_fixture.RandomId());
        var name = createdKey.CryptoKeyName;

        // Run the sample code.
        var result = _sample.UpdateKeyUpdateLabels(
            projectId: name.ProjectId, locationId: name.LocationId, keyRingId: name.KeyRingId, keyId: name.CryptoKeyId);

        Assert.NotEmpty(result.Labels);
        Assert.Equal("new_value", result.Labels["new_label"]);
    }
}
