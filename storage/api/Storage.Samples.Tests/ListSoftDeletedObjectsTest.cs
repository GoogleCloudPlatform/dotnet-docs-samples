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

using Google.Apis.Storage.v1.Data;
using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class ListSoftDeletedObjectsTest
{
    private readonly StorageFixture _fixture;

    public ListSoftDeletedObjectsTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListSoftDeletedObjects()
    {
        ListSoftDeletedObjectsSample listSoftDeletedObjects = new ListSoftDeletedObjectsSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        var objectNameOne = _fixture.GenerateName();
        var objectOneContent = _fixture.GenerateContent();
        var objectNameTwo = _fixture.GenerateName();
        var objectTwoContent = _fixture.GenerateContent();
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, objectNameOne, objectOneContent);
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, objectNameTwo, objectTwoContent);
        var preSoftDeleteObjects = _fixture.Client.ListObjects(bucketName);
        int preSoftDeleteObjectsCount = preSoftDeleteObjects.Count();
        _fixture.Client.DeleteObject(bucketName, objectNameOne);
        _fixture.Client.DeleteObject(bucketName, objectNameTwo);
        var softDeletedObjects = listSoftDeletedObjects.ListSoftDeletedObjects(bucketName);
        int softDeletedObjectsCount = softDeletedObjects.Count();
        Assert.Equal(preSoftDeleteObjectsCount, softDeletedObjectsCount);
        Assert.All(softDeletedObjects, AssertSoftDeletedObject);
        Assert.Multiple(
            () => Assert.Contains(softDeletedObjects, softDeletedObject => softDeletedObject.Name == objectNameOne),
            () => Assert.Contains(softDeletedObjects, softDeletedObject => softDeletedObject.Name == objectNameTwo)
        );
    }

    // Validates that the given object is soft-deleted.
    private void AssertSoftDeletedObject(Object o)
    {
        Assert.NotNull(o.Generation);
        Assert.NotNull(o.HardDeleteTimeDateTimeOffset);
        Assert.NotNull(o.SoftDeleteTimeDateTimeOffset);
    }
}
