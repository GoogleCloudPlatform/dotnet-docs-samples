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
using System.IO;
using Xunit;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class TransferBetweenPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly string _tempDirectory;
    private readonly string _tempDestinationDirectory;
    private readonly string _bucket;

    public TransferBetweenPosixTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _bucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_bucket);
        _tempDirectory = fixture.GenerateTempFolderPath();
        _tempDestinationDirectory = fixture.GenerateTempFolderPath();
    }

    [Fact]
    public void TransferBetweenPosix()
    {
        TransferBetweenPosixSample transferBetweenPosixSample = new TransferBetweenPosixSample();
        Directory.CreateDirectory(_tempDirectory);
        Directory.CreateDirectory(_tempDestinationDirectory);
        string fileName = Path.Combine(_tempDirectory, $"{Guid.NewGuid().ToString()}.txt");
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine(Guid.NewGuid().ToString());
        }
        var transferJob = transferBetweenPosixSample.TransferBetweenPosix(_fixture.ProjectId, _fixture.SourceAgentPoolName, _fixture.SinkAgentPoolName, _tempDirectory, _tempDestinationDirectory, _bucket);
        Assert.Contains("transferJobs/", transferJob.Name);
        _transferJobName = transferJob.Name;
        Assert.True(Directory.Exists(_tempDirectory));
        Assert.True(Directory.Exists(_tempDestinationDirectory));
        Assert.True(File.Exists(fileName));
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
            Directory.Delete(_tempDestinationDirectory, true);
            _fixture.Storage.DeleteBucket(_bucket);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
