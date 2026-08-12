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
public class ListFileArchivedGenerationTest
{
    private readonly StorageFixture _fixture;

    public ListFileArchivedGenerationTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListFileArchivedGeneration()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ListFileArchivedGenerationSample listFileArchivedGenerationSample = new ListFileArchivedGenerationSample();

        var objectName = _fixture.GenerateName();

        // Uploaded for the first time
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

        var testFiles = objects.Where(a => a.Name == objectName).ToList();

        Assert.Equal(2, testFiles.Count);
    }
}
