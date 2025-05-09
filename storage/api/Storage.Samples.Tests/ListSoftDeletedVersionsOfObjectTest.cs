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
using System.Collections.Generic;
using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class ListSoftDeletedVersionsOfObjectTest
{
    private readonly StorageFixture _fixture;
    private IList<long> _softDeleteObjectGenerations { get; } = new List<long>();

    public ListSoftDeletedVersionsOfObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListSoftDeletedVersionsOfObject()
    {
        int i = 3;
        int preSoftDeleteObjectVersionsCount = i;
        ListSoftDeletedVersionsOfObjectSample listSoftDeletedVersionOfObject = new ListSoftDeletedVersionsOfObjectSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        RestoreSoftDeletedObjectSample restoreSoftDeletedObjectSample = new RestoreSoftDeletedObjectSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        var objectName = _fixture.GenerateName();
        var objectContent = _fixture.GenerateContent();
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, objectName, objectContent);
        while (i >= 1)
        {
            var objectMetaData = getMetadataSample.GetMetadata(bucketName, objectName);
            _softDeleteObjectGenerations.Add(objectMetaData.Generation.Value);
            _fixture.Client.DeleteObject(bucketName, objectName);
            var restoredObject = restoreSoftDeletedObjectSample.RestoreSoftDeletedObject(bucketName, objectName, objectMetaData.Generation.Value);
            i--;
        }
        var softDeletedObjectVersions = listSoftDeletedVersionOfObject.ListSoftDeletedVersionsOfObject(bucketName, objectName);
        int softDeletedObjectVersionsCount = softDeletedObjectVersions.Count();
        Assert.Equal(preSoftDeleteObjectVersionsCount, softDeletedObjectVersionsCount);
        Assert.All(softDeletedObjectVersions, AssertSoftDeletedVersionsOfObject);
        Assert.Multiple(
            () => Assert.Contains(softDeletedObjectVersions, softDeletedObject => softDeletedObject.Name == objectName && softDeletedObject.Generation == _softDeleteObjectGenerations[_softDeleteObjectGenerations.Count - 1]),
            () => Assert.Contains(softDeletedObjectVersions, softDeletedObject => softDeletedObject.Name == objectName && softDeletedObject.Generation == _softDeleteObjectGenerations[_softDeleteObjectGenerations.Count - 2]),
            () => Assert.Contains(softDeletedObjectVersions, softDeletedObject => softDeletedObject.Name == objectName && softDeletedObject.Generation == _softDeleteObjectGenerations[_softDeleteObjectGenerations.Count - 3])
        );
        _fixture.Client.DeleteObject(bucketName, objectName);
    }

    private void AssertSoftDeletedVersionsOfObject(Object o)
    {
        Assert.NotNull(o.Generation);
        Assert.NotNull(o.HardDeleteTimeDateTimeOffset);
        Assert.NotNull(o.SoftDeleteTimeDateTimeOffset);
    }
}
