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
using Xunit.Abstractions;
using Xunit;
using System.IO;
using System.Linq;


namespace StorageTransfer.Samples.Tests;
[Collection(nameof(StorageFixture))]
public class TransferBetweenPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly ITestOutputHelper _outputHelper;
    public TransferBetweenPosixTest(StorageFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
    }

    [Fact]
    public void TransferBetweenPosix()
    {
        TransferBetweenPosixSample transferBetweenPosixSample = new TransferBetweenPosixSample(_outputHelper);
        Directory.CreateDirectory(_fixture.TempDirectory);
        Directory.CreateDirectory(_fixture.TempDestinationDirectory);
        string sourceDir =_fixture.RootDirectory;
        string[] txtList = Directory.GetFiles(sourceDir, "*.txt");
        // Copy one txt file.
        foreach (string f in txtList)
        {
            // Remove path from the file name.
            string fName = f.Split('/').Last();
            // Copy only one file from source directory to temp source directory. Will overwrite if the destination file already exists.
            File.Copy(Path.Combine(sourceDir, fName), Path.Combine(_fixture.TempDirectory, fName), true);
            break;
        }
        var storage = StorageClient.Create();
        var transferJob = transferBetweenPosixSample.TransferBetweenPosix(_fixture.ProjectId,_fixture.SourceAgentPoolName,_fixture.SinkAgentPoolName,_fixture.TempDirectory,_fixture.TempDestinationDirectory,_fixture.BucketNameSource);
        Assert.Contains("transferJobs/", transferJob.Name);
        Assert.True(Directory.Exists(_fixture.TempDirectory));
        Assert.True(Directory.Exists(_fixture.TempDestinationDirectory));
        Assert.True(File.Exists(txtList[0]));
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(45));
        string destinationFilePath = txtList[0].Replace(_fixture.RootDirectory.Substring(0,_fixture.RootDirectory.Length - 1),_fixture.TempDestinationDirectory);
        Assert.True(File.Exists(destinationFilePath));
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
            Directory.Delete(_fixture.TempDirectory, true);
            Directory.Delete(_fixture.TempDestinationDirectory, true);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
