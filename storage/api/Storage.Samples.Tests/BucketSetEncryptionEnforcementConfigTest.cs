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
public class BucketSetEncryptionEnforcementConfigTest
{
    private readonly StorageFixture _fixture;

    public BucketSetEncryptionEnforcementConfigTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void BucketSetEncryptionEnforcementConfig(
        bool enforceCmek,
        bool enforceGmek,
        bool enforceCsek)
    {
        var bucketSetEncConfigSample = new BucketSetEncryptionEnforcementConfigSample();
        var bucketName = _fixture.GenerateBucketName();
        string keyName = enforceCmek
            ? $"projects/{_fixture.ProjectId}/locations/{_fixture.KmsKeyLocation}/keyRings/{_fixture.KmsKeyRing}/cryptoKeys/{_fixture.KmsKeyName}"
            : null;
        _fixture.CreateBucket(bucketName: bucketName, location: _fixture.KmsKeyLocation);
        var bucketEncryptionData = bucketSetEncConfigSample.SetBucketEncryptionEnforcementConfig(
            bucketName: bucketName,
            kmsKeyName: keyName,
            enforceCmek: enforceCmek,
            enforceGmek: enforceGmek,
            enforceCsek: enforceCsek);

        string expectedCmek = (enforceGmek || enforceCsek) ? "FullyRestricted" : "NotRestricted";
        string expectedGmek = (enforceCmek || enforceCsek) ? "FullyRestricted" : "NotRestricted";
        string expectedCsek = (enforceCmek || enforceGmek) ? "FullyRestricted" : "NotRestricted";

        Assert.Multiple(() =>
        {
            Assert.Equal(expectedCmek, bucketEncryptionData.CustomerManagedEncryptionEnforcementConfig?.RestrictionMode);
            Assert.Equal(expectedCsek, bucketEncryptionData.CustomerSuppliedEncryptionEnforcementConfig?.RestrictionMode);
            Assert.Equal(expectedGmek, bucketEncryptionData.GoogleManagedEncryptionEnforcementConfig?.RestrictionMode);

            if (enforceCmek) Assert.Equal(keyName, bucketEncryptionData.DefaultKmsKeyName);
        });
    }
}
