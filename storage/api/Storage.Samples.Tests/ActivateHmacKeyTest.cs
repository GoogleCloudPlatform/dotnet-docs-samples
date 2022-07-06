// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

[Collection(nameof(StorageFixture))]
public class ActivateHmacKeyTest : HmacKeyManager
{
    public ActivateHmacKeyTest(StorageFixture fixture) : base(fixture)
    { }

    [Fact]
    public void TestActivateHmacKey()
    {
        CreateHmacKeySample createHmacKeySample = new CreateHmacKeySample();
        DeactivateHmacKeySample deactivateHmacKeySample = new DeactivateHmacKeySample();
        ActivateHmacKeySample activateHmacKeySample = new ActivateHmacKeySample();

        string serviceAccountEmail = _fixture.GetServiceAccountEmail();

        // Create key.
        var key = createHmacKeySample.CreateHmacKey(_fixture.ProjectId, serviceAccountEmail);
        _accessId = key.Metadata.AccessId;

        // Deactivate key.
        _fixture.HmacChangesPropagated.Eventually(() => deactivateHmacKeySample.DeactivateHmacKey(_fixture.ProjectId, _accessId));
        _isActive = false;

        // Activate key.
        var keyMetadata = _fixture.HmacChangesPropagated.Eventually(() => activateHmacKeySample.ActivateHmacKey(_fixture.ProjectId, _accessId));
        Assert.Equal("ACTIVE", keyMetadata.State);
        _isActive = true;
    }
}
