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

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageTransfer.V1;
using Xunit;

namespace StorageTransfer.Samples.Tests;
[Collection(nameof(StorageFixture))]
public class TransferBetweenPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    public TransferBetweenPosixTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TransferBetweenPosix()
    {
        TransferBetweenPosixSample transferBetweenPosixSample = new TransferBetweenPosixSample();
        Directory.CreateDirectory(_fixture.TempDirectory);
        Directory.CreateDirectory(_fixture.TempDestinationDirectory);
        string sourceDir = _fixture.RootDirectory;
        string fileName = Path.Combine(_fixture.TempDirectory, "test.txt");
        // Check if file already exists. If yes, delete it.
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            sw.WriteLine("test message");
        }
        var transferJob = transferBetweenPosixSample.TransferBetweenPosix(_fixture.ProjectId, _fixture.SourceAgentPoolName, _fixture.SinkAgentPoolName, _fixture.TempDirectory, _fixture.TempDestinationDirectory, _fixture.BucketNameSource);
        Assert.Contains("transferJobs/", transferJob.Name);
        _transferJobName = transferJob.Name;
        Assert.True(Directory.Exists(_fixture.TempDirectory));
        Assert.True(Directory.Exists(_fixture.TempDestinationDirectory));
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
            Directory.Delete(_fixture.TempDirectory, true);
            Directory.Delete(_fixture.TempDestinationDirectory, true);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
