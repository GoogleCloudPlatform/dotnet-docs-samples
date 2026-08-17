// Copyright 2021 Google Inc.
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

using Google.Cloud.Storage.V1;
using Xunit;

[Collection(nameof(StorageFixture))]
public class ChangeDefaultStorageClassTest
{
    private readonly StorageFixture _fixture;

    public ChangeDefaultStorageClassTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ChangeDefaultStorageClass()
    {
        ChangeDefaultStorageClassSample changeDefaultStorageClassSample = new ChangeDefaultStorageClassSample();

        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);

        // Change storage class to Coldline
        var bucket = changeDefaultStorageClassSample.ChangeDefaultStorageClass(bucketName, StorageClasses.Coldline);
        Assert.Equal(StorageClasses.Coldline, bucket.StorageClass);
    }
}
