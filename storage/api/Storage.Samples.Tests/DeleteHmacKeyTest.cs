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

[Collection(nameof(BucketFixture))]
public class DeleteHmacKeyTest
{
    private readonly BucketFixture _bucketFixture;

    public DeleteHmacKeyTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDeleteHmacKey()
    {
        CreateHmacKeySample createHmacKeySample = new CreateHmacKeySample();
        GetHmacKeySample getHmacKeySample = new GetHmacKeySample();
        DeactivateHmacKeySample deactivateHmacKeySample = new DeactivateHmacKeySample();
        DeleteHmacKeySample deleteHmacKeySample = new DeleteHmacKeySample();

        string serviceAccountEmail = _bucketFixture.GetServiceAccountEmail();

        // Create key.
        var key = createHmacKeySample.CreateHmacKey(_bucketFixture.ProjectId, serviceAccountEmail);

        // Deactivate key.
        deactivateHmacKeySample.DeactivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);

        // Delete key.
        deleteHmacKeySample.DeleteHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);

        // Get key.
        var keyMetadata = getHmacKeySample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal("DELETED", keyMetadata.State);
    }
}
