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

using Google.Apis.Storage.v1.Data;
using Xunit;

[Collection(nameof(StorageFixture))]
public class BucketUpdateEncryptionEnforcementConfigTest
{
    private readonly StorageFixture _fixture;

    public BucketUpdateEncryptionEnforcementConfigTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("FullyRestricted")]
    [InlineData(null)]
    public void BucketUpdateEncryptionEnforcementConfig(string restrictionMode)
    {
        var bucketSetEncConfigSample = new BucketSetEncryptionEnforcementConfigSample();
        var bucketUpdateEncConfigSample = new BucketUpdateEncryptionEnforcementConfigSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName: bucketName, location: _fixture.KmsKeyLocation);
        string keyName = $"projects/{_fixture.ProjectId}/locations/{_fixture.KmsKeyLocation}/keyRings/{_fixture.KmsKeyRing}/cryptoKeys/{_fixture.KmsKeyName}";

        bucketSetEncConfigSample.SetBucketEncryptionEnforcementConfig(
            bucketName: bucketName,
            kmsKeyName: keyName,
            enforceCmek: true);

        var encryptionData = new Bucket.EncryptionData
        {
            DefaultKmsKeyName = keyName,
            GoogleManagedEncryptionEnforcementConfig = restrictionMode != null
             ? new Bucket.EncryptionData.GoogleManagedEncryptionEnforcementConfigData
             { RestrictionMode = restrictionMode }
             : null
        };

        var bucketEncryptionData = bucketUpdateEncConfigSample.BucketUpdateEncryptionEnforcementConfig(bucketName, encryptionData);
        Assert.Equal(keyName, bucketEncryptionData.DefaultKmsKeyName);

        if (restrictionMode != null)
        {
            Assert.NotNull(encryptionData.GoogleManagedEncryptionEnforcementConfig);
            Assert.Equal(restrictionMode, encryptionData.GoogleManagedEncryptionEnforcementConfig.RestrictionMode);
        }
        else
        {
            Assert.Null(encryptionData.GoogleManagedEncryptionEnforcementConfig);
        }
    }
}
