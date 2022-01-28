﻿// Copyright 2021 Google Inc.
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
        ListFilesSample listFilesSample = new ListFilesSample();
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        DownloadFileSample downloadFileSample = new DownloadFileSample();
        ListFileArchivedGenerationSample listFileArchivedGenerationSample = new ListFileArchivedGenerationSample();
        DeleteFileArchivedGenerationSample deleteFileArchivedGenerationSample = new DeleteFileArchivedGenerationSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        var objectName = "HelloListFileArchivedGeneration.txt";

        // Enable bucket versioning
        bucketEnableVersioningSample.BucketEnableVersioning(_fixture.BucketNameGeneric);

        // Uploaded for the first time
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, objectName);

        // Upload again to archive previous generation.
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt", objectName);

        try
        {
            var objects = listFileArchivedGenerationSample.ListFileArchivedGeneration(_fixture.BucketNameGeneric);

            var testFiles = objects.Where(a => a.Name == objectName).ToList();
            
            Assert.Equal(2, testFiles.Count);            
            _fixture.CollectArchivedFiles(_fixture.BucketNameGeneric, objectName, testFiles[0].Generation);
            _fixture.CollectArchivedFiles(_fixture.BucketNameGeneric, objectName, testFiles[1].Generation);
        }
        finally
        {
            // Disable bucket versioning
            bucketDisableVersioningSample.BucketDisableVersioning(_fixture.BucketNameGeneric);
        }
    }
}
