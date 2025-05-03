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
public class TransferUsingManifestTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly string _rootDirectory;
    private readonly string _manifestObjectName;
    private readonly string _manifestBucket;
    private readonly string _sinkBucket;

    public TransferUsingManifestTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _manifestBucket = _fixture.GenerateBucketName();
        _sinkBucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_manifestBucket);
        _fixture.CreateBucketAndGrantStsPermissions(_sinkBucket);
        _rootDirectory = fixture.GetCurrentUserTempFolderPath();
        _manifestObjectName = $@"{Guid.NewGuid()}.csv";
        UploadObjectToManifestBucket(_manifestBucket);
    }

    [Fact]
    public void TransferUsingManifest()
    {
        TransferUsingManifestSample transferUsingManifestSample = new TransferUsingManifestSample();
        var transferJob = transferUsingManifestSample.TransferUsingManifest(_fixture.ProjectId, _fixture.SourceAgentPoolName, _rootDirectory, _manifestBucket, _sinkBucket, _manifestObjectName);
        Assert.Contains("transferJobs/", transferJob.Name);
        _transferJobName = transferJob.Name;
    }

    private void UploadObjectToManifestBucket(string bucketName)
    {
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($@"{Guid.NewGuid()}.jpeg");
        MemoryStream stream = new MemoryStream(byteArray);
        _fixture.Storage.UploadObject(bucketName, _manifestObjectName, "application/octet-stream", stream);
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
            _fixture.Storage.DeleteBucket(_manifestBucket, new DeleteBucketOptions { DeleteObjects = true });
            _fixture.Storage.DeleteBucket(_sinkBucket, new DeleteBucketOptions { DeleteObjects = true });
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
