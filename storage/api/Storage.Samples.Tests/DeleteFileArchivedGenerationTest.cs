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
        ListFilesSample listFilesSample = new ListFilesSample();
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        DownloadFileSample downloadFileSample = new DownloadFileSample();
        ListFileArchivedGenerationSample listFileArchivedGenerationSample = new ListFileArchivedGenerationSample();
        DeleteFileArchivedGenerationSample deleteFileArchivedGenerationSample = new DeleteFileArchivedGenerationSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        var objectName = "HelloDeleteFileArchivedGeneration.txt";

        // Enable bucket versioning
        bucketEnableVersioningSample.BucketEnableVersioning(_fixture.BucketNameGeneric);

        // Uploaded for the first time
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, objectName);

        // Upload again to archive previous generation.
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt", objectName);


        try
        {
            var objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameGeneric);

            Assert.Equal(2, objects.Count(a => a.Name == objectName));

            // Get Generations
            var testFiles = objects.Where(a => a.Name == objectName).ToList();
            long? fileArchivedGeneration = testFiles[0].Generation;
            long? fileCurrentGeneration = testFiles[1].Generation;

            // Delete first generation of the file
            deleteFileArchivedGenerationSample.DeleteFileArchivedGeneration(_fixture.BucketNameGeneric, objectName, fileArchivedGeneration);

            objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameGeneric);
            Assert.Equal(1, objects.Count(a => a.Name == objectName));

            // Delete second generation of the file
            deleteFileArchivedGenerationSample.DeleteFileArchivedGeneration(_fixture.BucketNameGeneric, objectName, fileCurrentGeneration);

            objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameGeneric);
            Assert.Equal(0, objects.Count(a => a.Name == objectName));
        }
        finally
        {
            // Disable bucket versioning
            bucketDisableVersioningSample.BucketDisableVersioning(_fixture.BucketNameGeneric);
        }
    }
}
