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

using System.Linq;
using Xunit;

[Collection(nameof(BucketFixture))]
public class HmacKeyTest
{
    private readonly BucketFixture _bucketFixture;

    public HmacKeyTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void HmacKeySampleTest()
    {
        CreateHmacKeySample createHmacKeySample = new CreateHmacKeySample();
        ListHmacKeysSample listHmacKeysSample = new ListHmacKeysSample();
        GetHmacKeySample hmacKeyGetSample = new GetHmacKeySample();
        DeactivateHmacKeySample hmacKeyDeactivateSample = new DeactivateHmacKeySample();
        ActivateHmacKeySample hmacKeyActivateSample = new ActivateHmacKeySample();
        DeleteHmacKeySample hmacKeyDeleteSample = new DeleteHmacKeySample();

        // These need to all run as one test so that we can use the created key in every test.
        _bucketFixture.DeleteAllHmacKeys();

        string serviceAccountEmail = _bucketFixture.GetServiceAccountEmail();

        // Create key.
        var key = createHmacKeySample.CreateHmacKey(_bucketFixture.ProjectId, serviceAccountEmail);
        Assert.Equal(key.Metadata.ServiceAccountEmail, serviceAccountEmail);

        // List keys.
        var keys = listHmacKeysSample.ListHmacKeys(_bucketFixture.ProjectId).ToList();
        Assert.True(keys.Count > 0);

        // Get key.
        var keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal(keyMetadata.ServiceAccountEmail, serviceAccountEmail);

        // Deactivate key.
        hmacKeyDeactivateSample.DeactivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal("INACTIVE", keyMetadata.State);

        // Activate key.
        hmacKeyActivateSample.ActivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keyMetadata = hmacKeyGetSample.GetHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        Assert.Equal("ACTIVE", keyMetadata.State);

        // Deactivate key.
        hmacKeyDeactivateSample.DeactivateHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);

        // Delete key.
        hmacKeyDeleteSample.DeleteHmacKey(_bucketFixture.ProjectId, key.Metadata.AccessId);
        keys = listHmacKeysSample.ListHmacKeys(_bucketFixture.ProjectId).ToList();
        Assert.True(keys.Count == 0);
    }
}
