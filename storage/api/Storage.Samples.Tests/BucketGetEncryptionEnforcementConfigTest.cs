// Copyright 2026 Google LLC
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
public class BucketGetEncryptionEnforcementConfigTest
{
    private readonly StorageFixture _fixture;

    public BucketGetEncryptionEnforcementConfigTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketGetEncryptionEnforcementConfig()
    {
        var bucketSetEncConfigSample = new BucketSetEncryptionEnforcementConfigSample();
        var bucketGetEncConfigSample = new BucketGetEncryptionEnforcementConfigSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName: bucketName, location: _fixture.KmsKeyLocation);

        string keyName = $"projects/{_fixture.ProjectId}/locations/{_fixture.KmsKeyLocation}/keyRings/{_fixture.KmsKeyRing}/cryptoKeys/{_fixture.KmsKeyName}";
        var bucket = bucketSetEncConfigSample.SetBucketEncryptionEnforcementConfig(
            bucketName: bucketName,
            kmsKeyName: keyName,
            enforceCmek: true);
        var bucketEncryptionData = bucketGetEncConfigSample.BucketGetEncryptionEnforcementConfig(bucket.Name);
        Assert.NotNull(bucketEncryptionData);
        Assert.Equal(keyName, bucketEncryptionData.DefaultKmsKeyName);
        Assert.Multiple(() =>
        {
            Assert.Equal("NotRestricted", bucketEncryptionData.CustomerManagedEncryptionEnforcementConfig?.RestrictionMode);
            Assert.Equal("FullyRestricted", bucketEncryptionData.CustomerSuppliedEncryptionEnforcementConfig?.RestrictionMode);
            Assert.Equal("FullyRestricted", bucketEncryptionData.GoogleManagedEncryptionEnforcementConfig?.RestrictionMode);
        });
    }
}
