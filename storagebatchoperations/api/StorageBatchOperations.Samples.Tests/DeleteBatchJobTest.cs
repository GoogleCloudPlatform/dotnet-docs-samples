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
public class DeleteBatchJobTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public DeleteBatchJobTest(StorageFixture fixture)
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
    public void TestDeleteBatchJob()
    {
        DeleteBatchJobSample deleteBatchJob = new DeleteBatchJobSample();
        ListBatchJobsSample listBatchJobs = new ListBatchJobsSample();
        GetBatchJobSample getBatchJob = new GetBatchJobSample();
        CreateBatchJobSample createBatchJob = new CreateBatchJobSample();

        string filter = "";
        int pageSize = 10;
        string orderBy = "create_time";

        var jobId = _fixture.GenerateGuid();
        var createdJob = createBatchJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        // Delete the created job.
        deleteBatchJob.DeleteBatchJob(createdJob.Name);
        var batchJobs = listBatchJobs.ListBatchJobs(_fixture.LocationName, filter, pageSize, orderBy);
        // Verify that the job is deleted.
        Assert.DoesNotContain(batchJobs, job => job.JobName == createdJob.JobName);
        // Attempt to get the deleted job, which should throw an exception.
        var exception = Assert.Throws<Grpc.Core.RpcException>(() => getBatchJob.GetBatchJob(createdJob.Name));
        Assert.Equal(Grpc.Core.StatusCode.NotFound, exception.StatusCode);
    }
}
