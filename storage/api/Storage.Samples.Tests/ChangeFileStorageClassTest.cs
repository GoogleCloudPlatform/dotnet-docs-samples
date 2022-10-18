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
public class ChangeFileStorageClassTest
{
    private readonly StorageFixture _fixture;

    public ChangeFileStorageClassTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ChangeFileStorageClass()
    {
        ChangeFileStorageClassSample changeFileStorageClassSample = new ChangeFileStorageClassSample();

        // Change storage class to Coldline
        var file = changeFileStorageClassSample.ChangeFileStorageClass(_fixture.BucketNameGeneric, _fixture.FileName, StorageClasses.Coldline);
        Assert.Equal(StorageClasses.Coldline, file.StorageClass);

        // Change it back to standard
        changeFileStorageClassSample.ChangeFileStorageClass(_fixture.BucketNameGeneric, _fixture.FileName, StorageClasses.Standard);
    }
}
