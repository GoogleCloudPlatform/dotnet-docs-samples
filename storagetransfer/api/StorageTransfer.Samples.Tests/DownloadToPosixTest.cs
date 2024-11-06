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
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace StorageTransfer.Samples.Tests;
[Collection(nameof(StorageFixture))]
public class DownloadToPosixTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private string _transferJobName;
    private readonly ITestOutputHelper _outputHelper;
    public DownloadToPosixTest(StorageFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
    }

    [Fact]
    public void DownloadToPosix()
    {
        DownloadToPosixSample downloadToPosixSample = new DownloadToPosixSample(_outputHelper);
        Directory.CreateDirectory(_fixture.TempDirectory);
        var storage = StorageClient.Create();
        byte[] byteArray = Encoding.UTF8.GetBytes("flower.jpeg");
        MemoryStream stream = new MemoryStream(byteArray);
        string fileName = $"{_fixture.GcsSourcePath}{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
        string filePath = $"{_fixture.TempDirectory}/{fileName.Split('/').Last()}";
        storage.UploadObject(_fixture.BucketNameSource,fileName, "application/octet-stream", stream);
        var transferJob = downloadToPosixSample.DownloadToPosix(_fixture.ProjectId,_fixture.SinkAgentPoolName,_fixture.BucketNameSource,_fixture.GcsSourcePath,_fixture.TempDirectory);
        Assert.Contains("transferJobs/", transferJob.Name);
        Assert.True( Directory.Exists(_fixture.TempDirectory));
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(45));
        Assert.True( File.Exists(filePath));
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
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
