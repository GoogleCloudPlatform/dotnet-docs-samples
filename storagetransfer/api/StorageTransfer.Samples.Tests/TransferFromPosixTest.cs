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
public class TransferFromPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly string _rootDirectory;
    private readonly string _bucket;

    public TransferFromPosixTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _bucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_bucket);
        _rootDirectory = fixture.GetCurrentUserTempFolderPath();
    }

    [Fact]
    public void TransferFromPosix()
    {
        TransferFromPosixSample transferFromPosixSample = new TransferFromPosixSample();
        var transferJob = transferFromPosixSample.TransferFromPosix(_fixture.ProjectId, _fixture.SourceAgentPoolName, _rootDirectory, _bucket);
        Assert.Contains("transferJobs/", transferJob.Name);
        _transferJobName = transferJob.Name;
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
            _fixture.Storage.DeleteBucket(_bucket);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
