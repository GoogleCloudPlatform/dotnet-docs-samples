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
using Xunit;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class CheckLatestTransferOperationTest
{
    private readonly StorageFixture _fixture;
    private string _jobName;
    private readonly string _transferJobName;

    public CheckLatestTransferOperationTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _fixture.BucketNameSource = _fixture.GenerateBucketName();
        _fixture.BucketNameSink = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_fixture.BucketNameSource);
        _fixture.CreateBucketAndGrantStsPermissions(_fixture.BucketNameSink);
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
        // Initialize request argument(s)
        TransferJob transferJob = new TransferJob
        {
            ProjectId = _fixture.ProjectId,
            TransferSpec = new TransferSpec
            {
                GcsDataSink = new GcsData { BucketName = _fixture.BucketNameSource },
                GcsDataSource = new GcsData { BucketName = _fixture.BucketNameSink }
            },
            Status = TransferJob.Types.Status.Enabled
        };
        CreateTransferJobRequest request = new CreateTransferJobRequest
        {
            TransferJob = transferJob
        };
        // Make the request
        TransferJob response = _fixture.Sts.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });
        return response.Name;
    }
}
