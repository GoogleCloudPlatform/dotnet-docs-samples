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
public class ListBatchJobsTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public ListBatchJobsTest(StorageFixture fixture)
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
    public void ListBatchJobs()
    {
        ListBatchJobsSample listBatchJobs = new ListBatchJobsSample();
        string filter = "state:succeeded";
        int pageSize = 10;
        string orderBy = "create_time";
        var jobId = _fixture.GenerateJobId();
        CreateBatchJobSample createBatchJob = new CreateBatchJobSample();
        var createdJob = createBatchJob.CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        var batchJobs = listBatchJobs.ListBatchJobs(_fixture.LocationName, filter, pageSize, orderBy);
        Assert.Contains(batchJobs, job => job.JobName == createdJob.JobName && job.State == createdJob.State && job.SourceCase == createdJob.SourceCase && job.TransformationCase == createdJob.TransformationCase);
        Assert.All(batchJobs, AssertBatchJob);
        _fixture.DeleteBatchJob(createdJob.Name);
    }

    private void AssertBatchJob(Job b)
    {
        Assert.NotNull(b.Name);
        Assert.NotNull(b.JobName);
        Assert.NotNull(b.CreateTime);
        Assert.NotNull(b.CompleteTime);
        Assert.NotNull(b.SourceCase.ToString());
        Assert.NotNull(b.TransformationCase.ToString());
    }
}
