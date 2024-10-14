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
using Xunit;
using Xunit.Abstractions;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class CheckLatestTransferOperationTest
{

    private readonly StorageFixture _fixture;
    private string _jobName;
    private readonly ITestOutputHelper _outputHelper;
    public CheckLatestTransferOperationTest(StorageFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _fixture = fixture;
    }

    [Fact]
    public void CheckLatestTransferOperation()
    {
        CheckLatestTransferOperationSample checkLatestTransferOperationSample = new CheckLatestTransferOperationSample(_outputHelper);
        var transferJob = checkLatestTransferOperationSample.CheckLatestTransferOperation(_fixture.ProjectId,_fixture.JobName);
        Assert.Contains("transferJobs/", transferJob.Name);
        _jobName = transferJob.Name;
    }
}
