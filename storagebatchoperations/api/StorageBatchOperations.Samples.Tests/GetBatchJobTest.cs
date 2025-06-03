// Copyright 2025 Google LLC
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

using Google.Cloud.StorageBatchOperations.V1;
using Xunit;

[Collection(nameof(StorageFixture))]
public class GetBatchJobTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public GetBatchJobTest(StorageFixture fixture)
    {
        _fixture = fixture;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        _bucket = new BucketList.Types.Bucket
        {
            Bucket_ = bucketName,
            PrefixList = _prefixListObject
        };
        _bucketList.Buckets.Insert(0, _bucket);
    }

    [Fact]
    public void GetBatchJob()
    {
        GetBatchJobSample getJob = new GetBatchJobSample();
        var jobId = _fixture.GenerateJobId();
        CreateBatchJobSample createJob = new CreateBatchJobSample();
        var createdJob = createJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        var getCreatedJob = getJob.GetBatchJob(createdJob.Name);
        Assert.Equal(createdJob.Name, getCreatedJob.Name);
        Assert.Equal(createdJob.BucketList, getCreatedJob.BucketList);
        Assert.Equal(createdJob.TransformationCase.ToString(), getCreatedJob.TransformationCase.ToString());
        Assert.Equal(createdJob.SourceCase.ToString(), getCreatedJob.SourceCase.ToString());
        Assert.Equal(createdJob.State, getCreatedJob.State);
        Assert.Equal(createdJob.Description, getCreatedJob.Description);
        Assert.Equal(createdJob.ScheduleTime, getCreatedJob.ScheduleTime);
        Assert.Equal(createdJob.CompleteTime, getCreatedJob.CompleteTime);
        Assert.Equal(createdJob.CreateTime, getCreatedJob.CreateTime);
        Assert.Equal(createdJob.Counters, getCreatedJob.Counters);
        _fixture.DeleteBatchJob(createdJob.Name);
    }
}
