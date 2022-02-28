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

using System;
using System.IO;
using Xunit;

[Collection(nameof(StorageFixture))]
public class CopyFileArchivedGenerationTest
{
    private readonly StorageFixture _fixture;

    public CopyFileArchivedGenerationTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CopyFileArchivedGeneration()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        DownloadFileSample downloadFileSample = new DownloadFileSample();
        CopyFileArchivedGenerationSample copyFileArchivedGenerationSample = new CopyFileArchivedGenerationSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        var objectName = Guid.NewGuid().ToString() +".txt";
        var copiedObjectName = Guid.NewGuid().ToString() + ".txt";

        // Enable bucket versioning
        bucketEnableVersioningSample.BucketEnableVersioning(_fixture.BucketNameGeneric);

        // Uploaded for the first time
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, objectName);

        // Get generation of first version of the file
        var obj = getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, objectName);
        var fileArchivedGeneration = obj.Generation;

        // Upload again to archive previous generation.
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt", objectName);

        // Get generation of second version of the file
        obj = getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, objectName);
        var fileCurrentGeneration = obj.Generation;


        _fixture.CollectArchivedFiles(_fixture.BucketNameGeneric, objectName, fileArchivedGeneration);
        _fixture.CollectArchivedFiles(_fixture.BucketNameGeneric, objectName, fileCurrentGeneration);

        try
        {
            // Copy first version of the file to new bucket.
            copyFileArchivedGenerationSample.CopyFileArchivedGeneration(_fixture.BucketNameGeneric, objectName,
                _fixture.BucketNameRegional, _fixture.CollectRegionalObject(copiedObjectName), fileArchivedGeneration);

            // Download copied file
            downloadFileSample.DownloadFile(_fixture.BucketNameRegional, copiedObjectName, copiedObjectName);

            // Match file contents with first version of the file
            Assert.Equal(File.ReadAllText(_fixture.FilePath), File.ReadAllText(copiedObjectName));
        }
        finally
        {
            File.Delete(copiedObjectName);

            // Disable bucket versioning
            bucketDisableVersioningSample.BucketDisableVersioning(_fixture.BucketNameGeneric);
        }
    }
}
