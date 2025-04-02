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

using Google;
using Xunit;

[Collection(nameof(StorageFixture))]
public class MoveObjectTest
{
    private readonly StorageFixture _fixture;

    public MoveObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    // Moves the source object to destination object within a hierarchical namespace enabled bucket.
    [Fact]
    public void MoveObject()
    {
        MoveObjectSample moveObjectSample = new MoveObjectSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ListFilesSample listFilesSample = new ListFilesSample();
        uploadObjectFromMemory.UploadObjectFromMemory(_fixture.BucketNameHns, "CFile.txt", "Hello World");
        var uploadedFileMetadata = getMetadataSample.GetMetadata(_fixture.BucketNameHns, "CFile.txt");
        // Make sure the destination object doesn't exist until we move it there.
        Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_fixture.BucketNameHns, "Copy.txt"));
        moveObjectSample.MoveObject(_fixture.BucketNameHns, uploadedFileMetadata.Name, "Copy.txt");
        var files = listFilesSample.ListFiles(_fixture.BucketNameHns);
        // Make sure the source object doesn't exist after move operation is performed.
        Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_fixture.BucketNameHns, uploadedFileMetadata.Name));
        Assert.Contains(files, c => c.Name == "Copy.txt");
    }

    // Moves the source object to destination object within hns bucket with correct preconditions set.
    [Fact]
    public void MoveObjectWithCorrectDestinationObjectGeneration()
    {
        MoveObjectSample moveObjectSample = new MoveObjectSample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ListFilesSample listFilesSample = new ListFilesSample();
        uploadObjectFromMemory.UploadObjectFromMemory(_fixture.BucketNameHns, "CopyFile.txt", "Hello World");
        var uploadedFileMetadata = getMetadataSample.GetMetadata(_fixture.BucketNameHns, "CopyFile.txt");
        moveObjectSample.MoveObjectWithOptions(_fixture.BucketNameHns, _fixture.FileName, "CopyFile.txt", uploadedFileMetadata.Generation);
        var files = listFilesSample.ListFiles(_fixture.BucketNameHns);
        // Make sure the source object doesn't exist after move operation is performed.
        Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_fixture.BucketNameHns, _fixture.FileName));
        Assert.Contains(files, c => c.Name == "CopyFile.txt");
    }
}
