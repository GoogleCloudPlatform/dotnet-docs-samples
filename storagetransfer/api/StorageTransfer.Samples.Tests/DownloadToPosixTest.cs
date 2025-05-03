// Copyright 2024 Google LLC
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

using Google.Cloud.Storage.V1;
using Google.Cloud.StorageTransfer.V1;
using System;
using System.IO;
using Xunit;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class DownloadToPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly string _tempDirectory;
    private readonly string _gcsSourcePath;
    private readonly string _sourceBucket;

    public DownloadToPosixTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _sourceBucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_sourceBucket);
        _tempDirectory = _fixture.GenerateTempFolderPath();
        _gcsSourcePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}/";
        UploadObjectToBucket(_sourceBucket);
    }

    [Fact]
    public void DownloadToPosix()
    {
        DownloadToPosixSample downloadToPosixSample = new DownloadToPosixSample();
        Directory.CreateDirectory(_tempDirectory);
        var transferJob = downloadToPosixSample.DownloadToPosix(_fixture.ProjectId, _fixture.SinkAgentPoolName, _sourceBucket, _gcsSourcePath, _tempDirectory);
        Assert.Contains("transferJobs/", transferJob.Name);
        Assert.True(Directory.Exists(_tempDirectory));
        _transferJobName = transferJob.Name;
    }

    private void UploadObjectToBucket(string bucketName)
    {
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($@"{Guid.NewGuid()}.jpeg");
        MemoryStream stream = new MemoryStream(byteArray);
        string fileName = $"{_gcsSourcePath}{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        _fixture.Storage.UploadObject(bucketName, fileName, "application/octet-stream", stream);
    }

    public void Dispose()
    {
        try
        {
            _fixture.Sts.UpdateTransferJob(new UpdateTransferJobRequest()
            {
                ProjectId = _fixture.ProjectId,
                JobName = _transferJobName,
                TransferJob = new TransferJob()
                {
                    Name = _transferJobName,
                    Status = TransferJob.Types.Status.Deleted
                }
            });
            Directory.Delete(_tempDirectory, true);
            _fixture.Storage.DeleteBucket(_sourceBucket, new DeleteBucketOptions { DeleteObjects = true });
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
