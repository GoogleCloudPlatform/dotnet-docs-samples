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

using System.IO;
using Xunit;

[Collection(nameof(BucketFixture))]
public class CopyFileArchivedGenerationTest
{
    private readonly BucketFixture _bucketFixture;

    public CopyFileArchivedGenerationTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void CopyFileArchivedGeneration()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        DownloadFileSample downloadFileSample = new DownloadFileSample();
        CopyFileArchivedGenerationSample copyFileArchivedGenerationSample = new CopyFileArchivedGenerationSample();
        DeleteFileArchivedGenerationSample deleteFileArchivedGenerationSample = new DeleteFileArchivedGenerationSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        var objectName = "HelloCopyArchive.txt";
        var copiedObjectName = "ByeCopy.txt";

        // Enable bucket versioning
        bucketEnableVersioningSample.BucketEnableVersioning(_bucketFixture.BucketNameGeneric);

        // Uploaded for the first time
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, objectName);

        // Get generation of first version of the file
        var obj = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, objectName);
        var fileArchivedGeneration = obj.Generation;

        // Upload again to archive previous generation.
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt", objectName);

        // Get generation of second version of the file
        obj = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, objectName);
        var fileCurrentGeneration = obj.Generation;


        _bucketFixture.CollectArchivedFiles(_bucketFixture.BucketNameGeneric, objectName, fileArchivedGeneration);
        _bucketFixture.CollectArchivedFiles(_bucketFixture.BucketNameGeneric, objectName, fileCurrentGeneration);

        try
        {
            // Copy first version of the file to new bucket.
            copyFileArchivedGenerationSample.CopyFileArchivedGeneration(_bucketFixture.BucketNameGeneric, objectName,
                _bucketFixture.BucketNameRegional, _bucketFixture.CollectRegionalObject(copiedObjectName), fileArchivedGeneration);

            // Download copied file
            downloadFileSample.DownloadFile(_bucketFixture.BucketNameRegional, copiedObjectName, copiedObjectName);

            // Match file contents with first version of the file
            Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText(copiedObjectName));
        }
        finally
        {
            File.Delete(copiedObjectName);

            // Disable bucket versioning
            bucketDisableVersioningSample.BucketDisableVersioning(_bucketFixture.BucketNameGeneric);
        }
    }
}
