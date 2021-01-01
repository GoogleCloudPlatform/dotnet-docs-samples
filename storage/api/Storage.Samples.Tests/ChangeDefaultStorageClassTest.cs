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

[Collection(nameof(BucketFixture))]
public class ChangeDefaultStorageClassTest
{
    private readonly BucketFixture _bucketFixture;

    public ChangeDefaultStorageClassTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void ChangeDefaultStorageClass()
    {
        ChangeDefaultStorageClassSample changeDefaultStorageClassSample = new ChangeDefaultStorageClassSample();

        // Change storage class to Coldline
        var bucket = changeDefaultStorageClassSample.ChangeDefaultStorageClass(_bucketFixture.BucketNameGeneric, StorageClasses.Coldline);
        Assert.Equal(StorageClasses.Coldline, bucket.StorageClass);

        // Change it back to standard
        changeDefaultStorageClassSample.ChangeDefaultStorageClass(_bucketFixture.BucketNameGeneric, StorageClasses.Standard);
    }
}
