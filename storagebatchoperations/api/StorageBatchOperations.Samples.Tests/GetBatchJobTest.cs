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
            // The prefix list is used to specify the objects to be deleted. To match all objects, use an empty list.
            PrefixList = _prefixListObject
        };
        // Adding the bucket to the bucket list.
        _bucketList.Buckets.Insert(0, _bucket);
    }

    [Fact]
    public void TestGetBatchJob()
    {
        GetBatchJobSample getJob = new GetBatchJobSample();
        CreateBatchJobSample createJob = new CreateBatchJobSample();

        var jobId = _fixture.GenerateGuid();
        // Create a batch job with the specified bucket list and job ID.
        var createdJob = createJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        // Get the created job using its name.
        var retrievedJob = getJob.GetBatchJob(createdJob.Name);
        // Assert that the retrieved job is not null.
        Assert.NotNull(retrievedJob);
        // Assert that the retrieved job's metadata matches with the created job's metadata.
        Assert.Equal(createdJob.Name, retrievedJob.Name);
        Assert.Equal(createdJob.BucketList, retrievedJob.BucketList);
        Assert.Equal(createdJob.TransformationCase.ToString(), retrievedJob.TransformationCase.ToString());
        Assert.Equal(createdJob.SourceCase.ToString(), retrievedJob.SourceCase.ToString());
        Assert.Equal(createdJob.State, retrievedJob.State);
        Assert.Equal(createdJob.Description, retrievedJob.Description);
        Assert.Equal(createdJob.ScheduleTime, retrievedJob.ScheduleTime);
        Assert.Equal(createdJob.CompleteTime, retrievedJob.CompleteTime);
        Assert.Equal(createdJob.CreateTime, retrievedJob.CreateTime);
        Assert.Equal(createdJob.Counters, retrievedJob.Counters);
        _fixture.DeleteBatchJob(createdJob.Name);
    }
}
