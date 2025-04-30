// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class RestoreSoftDeletedObjectTest
{
    private readonly StorageFixture _fixture;

    public RestoreSoftDeletedObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void RestoreSoftDeletedObject()
    {
        RestoreSoftDeletedObjectSample restoreSoftDeletedObjectSample = new RestoreSoftDeletedObjectSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var objectName = _fixture.GenerateName();
        var objectContent = _fixture.GenerateContent();
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, objectName, objectContent);
        var objectMetaData = getMetadataSample.GetMetadata(bucketName, objectName);
        _fixture.Client.DeleteObject(bucketName, objectName);
        var restoredObject = restoreSoftDeletedObjectSample.RestoreSoftDeletedObject(bucketName, objectName, objectMetaData.Generation.Value);
        Assert.Equal(objectName, restoredObject.Name);
        _fixture.Client.DeleteObject(bucketName, objectName);
    }
}
