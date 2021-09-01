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
public class DeleteHmacKeyTest : HmacKeyManager
{
    public DeleteHmacKeyTest(StorageFixture fixture) : base(fixture)
    { }

    [Fact]
    public void TestDeleteHmacKey()
    {
        CreateHmacKeySample createHmacKeySample = new CreateHmacKeySample();
        GetHmacKeySample getHmacKeySample = new GetHmacKeySample();
        DeactivateHmacKeySample deactivateHmacKeySample = new DeactivateHmacKeySample();
        DeleteHmacKeySample deleteHmacKeySample = new DeleteHmacKeySample();

        string serviceAccountEmail = _fixture.GetServiceAccountEmail();

        // Create key.
        var key = createHmacKeySample.CreateHmacKey(_fixture.ProjectId, serviceAccountEmail);
        _accessId = key.Metadata.AccessId;

        // Deactivate key.
        _fixture.HmacChangesPropagated.Eventually(() => deactivateHmacKeySample.DeactivateHmacKey(_fixture.ProjectId, _accessId));
        _isActive = false;

        // Delete key.
        _fixture.HmacChangesPropagated.Eventually(() => deleteHmacKeySample.DeleteHmacKey(_fixture.ProjectId, _accessId));

        // Get key.
        _fixture.HmacChangesPropagated.Eventually(() =>
        {
            var keyMetadata = getHmacKeySample.GetHmacKey(_fixture.ProjectId, _accessId);
            Assert.Equal("DELETED", keyMetadata.State);
        });
        _accessId = null;
    }
}
