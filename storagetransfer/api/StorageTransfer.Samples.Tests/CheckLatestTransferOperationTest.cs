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

using Google.Cloud.StorageTransfer.V1;
using System;
using Xunit;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class CheckLatestTransferOperationTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _jobName;
    private readonly string _transferJobName;
    private readonly string _sourceBucket;
    private readonly string _sinkBucket;

    public CheckLatestTransferOperationTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _sourceBucket = _fixture.GenerateBucketName();
        _sinkBucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_sourceBucket);
        _fixture.CreateBucketAndGrantStsPermissions(_sinkBucket);
        _transferJobName = CreateTransferJob();
    }

    [Fact]
    public void CheckLatestTransferOperation()
    {
        CheckLatestTransferOperationSample checkLatestTransferOperationSample = new CheckLatestTransferOperationSample();
        var transferJob = checkLatestTransferOperationSample.CheckLatestTransferOperation(_fixture.ProjectId, _transferJobName);
        Assert.Contains("transferJobs/", transferJob.Name);
        _jobName = transferJob.Name;
    }

    private string CreateTransferJob()
    {
        TransferJob transferJob = new TransferJob
        {
            ProjectId = _fixture.ProjectId,
            TransferSpec = new TransferSpec
            {
                GcsDataSink = new GcsData { BucketName = _sinkBucket },
                GcsDataSource = new GcsData { BucketName = _sourceBucket }
            },
            Status = TransferJob.Types.Status.Enabled
        };
        CreateTransferJobRequest request = new CreateTransferJobRequest
        {
            TransferJob = transferJob
        };

        TransferJob response = _fixture.Sts.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });
        return response.Name;
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
            _fixture.Storage.DeleteBucket(_sourceBucket);
            _fixture.Storage.DeleteBucket(_sinkBucket);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
