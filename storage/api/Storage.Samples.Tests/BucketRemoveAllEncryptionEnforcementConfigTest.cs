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
public class BucketRemoveAllEncryptionEnforcementConfigTest
{
    private readonly StorageFixture _fixture;

    public BucketRemoveAllEncryptionEnforcementConfigTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketRemoveAllEncryptionEnforcementConfig()
    {
        var bucketSetEncConfigSample = new BucketSetEncryptionEnforcementConfigSample();
        var bucketRemoveEncConfigSample = new BucketRemoveAllEncryptionEnforcementConfigSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName: bucketName, location: _fixture.KmsKeyLocation);
        string keyName = $"projects/{_fixture.ProjectId}/locations/{_fixture.KmsKeyLocation}/keyRings/{_fixture.KmsKeyRing}/cryptoKeys/{_fixture.KmsKeyName}";
        var bucket = bucketSetEncConfigSample.SetBucketEncryptionEnforcementConfig(
            bucketName: bucketName,
            kmsKeyName: keyName,
            enforceCmek: true);
        var updatedBucket = bucketRemoveEncConfigSample.BucketRemoveAllEncryptionEnforcementConfig(bucket.Name);
        Assert.Equal(updatedBucket.Encryption.DefaultKmsKeyName, bucket.Encryption.DefaultKmsKeyName);
        Assert.Multiple(() =>
        {
            Assert.Null(updatedBucket.Encryption.CustomerSuppliedEncryptionEnforcementConfig);
            Assert.Null(updatedBucket.Encryption.CustomerManagedEncryptionEnforcementConfig);
            Assert.Null(updatedBucket.Encryption.GoogleManagedEncryptionEnforcementConfig);
        });
    }
}
