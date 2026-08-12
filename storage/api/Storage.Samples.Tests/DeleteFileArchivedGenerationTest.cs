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

using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class DeleteFileArchivedGenerationTest
{
    private readonly StorageFixture _fixture;

    public DeleteFileArchivedGenerationTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DeleteFileArchivedGeneration()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ListFileArchivedGenerationSample listFileArchivedGenerationSample = new ListFileArchivedGenerationSample();
        DeleteFileArchivedGenerationSample deleteFileArchivedGenerationSample = new DeleteFileArchivedGenerationSample();

        var objectName = _fixture.GenerateName();

        uploadFileSample.UploadFile(_fixture.BucketNameVersioned, _fixture.FilePath, objectName);

        // Get generation of first version of the file
        var obj = getMetadataSample.GetMetadata(_fixture.BucketNameVersioned, objectName);
        var fileArchivedGeneration = obj.Generation;

        _fixture.CollectArchivedFiles(_fixture.BucketNameVersioned, objectName, fileArchivedGeneration);
        // Upload again to archive previous generation.
        uploadFileSample.UploadFile(_fixture.BucketNameVersioned, "Resources/HelloDownloadCompleteByteRange.txt", objectName);

        // Get generation of second version of the file
        obj = getMetadataSample.GetMetadata(_fixture.BucketNameVersioned, objectName);
        var fileCurrentGeneration = obj.Generation;

        _fixture.CollectArchivedFiles(_fixture.BucketNameVersioned, objectName, fileCurrentGeneration);
        var objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameVersioned);

        Assert.Equal(2, objects.Count(a => a.Name == objectName));

        // Delete first generation of the file
        deleteFileArchivedGenerationSample.DeleteFileArchivedGeneration(_fixture.BucketNameVersioned, objectName, fileArchivedGeneration);

        objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameVersioned);
        Assert.Equal(1, objects.Count(a => a.Name == objectName));

        // Delete second generation of the file
        deleteFileArchivedGenerationSample.DeleteFileArchivedGeneration(_fixture.BucketNameVersioned, objectName, fileCurrentGeneration);

        objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameVersioned);
        Assert.Equal(0, objects.Count(a => a.Name == objectName));
    }
}
