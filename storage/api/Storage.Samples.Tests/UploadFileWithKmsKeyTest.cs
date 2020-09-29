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
public class UploadFileWithKmsKeyTest
{
    private readonly BucketFixture _bucketFixture;

    public UploadFileWithKmsKeyTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void UploadFileWithKmsKey()
    {
        UploadFileWithKmsKeySample uploadFileWithKmsKeySample = new UploadFileWithKmsKeySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        // upload file with kms key
        uploadFileWithKmsKeySample.UploadFileWithKmsKey(_bucketFixture.ProjectId, _bucketFixture.BucketNameRegional,
            _bucketFixture.KmsKeyLocation, _bucketFixture.KmsKeyRing, _bucketFixture.KmsKeyName, _bucketFixture.FilePath, "HelloUploadWithKmsKey.txt");

        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketNameRegional, "HelloUploadWithKmsKey.txt");
        Assert.Contains(_bucketFixture.KmsKeyName, metadata.KmsKeyName);
    }
}
