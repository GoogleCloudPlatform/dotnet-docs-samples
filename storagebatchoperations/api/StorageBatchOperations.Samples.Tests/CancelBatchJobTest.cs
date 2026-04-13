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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageBatchOperations.V1;
using Google.LongRunning;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

[Collection(nameof(StorageFixture))]
public class CancelBatchJobTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket;
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public CancelBatchJobTest(StorageFixture fixture)
    {
        int i = 20;
        _fixture = fixture;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        // Uploading objects to the bucket.
        while (i >= 0)
        {
            var objectName = _fixture.GenerateGuid();
            var objectContent = _fixture.GenerateGuid();
            byte[] byteObjectContent = Encoding.UTF8.GetBytes(objectContent);
            MemoryStream streamObjectContent = new MemoryStream(byteObjectContent);
            _fixture.Client.UploadObject(bucketName, objectName, "application/text", streamObjectContent);
            i--;
        }

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
    public void TestCancelBatchJob()
    {
        CancelBatchJobSample cancelBatchJob = new CancelBatchJobSample();
        ListBatchJobsSample listBatchJobs = new ListBatchJobsSample();
        GetBatchJobSample getBatchJob = new GetBatchJobSample();

        string filter = "state:canceled";
        int pageSize = 10;
        string orderBy = "create_time";

        var jobId = _fixture.GenerateGuid();
        var createdJobName = CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        Assert.NotNull(createdJobName);

        // Poll until the job is in a state that can be cancelled (Running)
        // This prevents attempting to cancel a job that hasn't started or has already finished.
        bool isCancellable = false;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Job currentJob = getBatchJob.GetBatchJob(createdJobName);
            if (currentJob.State == Job.Types.State.Running)
            {
                isCancellable = true;
                break;
            }
            if (currentJob.State == Job.Types.State.Succeeded) break;
        }

        Assert.True(isCancellable, "Job did not reach a Running state in time to be cancelled.");
        cancelBatchJob.CancelBatchJob(createdJobName);

        Job finalJob = null;
        for (int attempt = 0; attempt < 1000; attempt++)
        {
            finalJob = getBatchJob.GetBatchJob(createdJobName);
            if (finalJob.State == Job.Types.State.Canceled) break;
        }
        Assert.Equal(createdJobName, finalJob.Name.ToString());
        Assert.Equal(Job.Types.State.Canceled, finalJob.State);

        var batchJobs = listBatchJobs.ListBatchJobs(_fixture.LocationName, filter, pageSize, orderBy);
        Assert.Contains(batchJobs, j => j.Name == createdJobName);
        _fixture.DeleteBatchJob(createdJobName);
    }

    /// <summary>
    /// Create a batch job with the specified transformation case and bucket list.
    /// </summary>
    public static string CreateBatchJob(LocationName locationName,
        BucketList bucketList,
        string jobId = "12345678910")
    {
        StorageBatchOperationsClient storageBatchClient = StorageBatchOperationsClient.Create();

        // Creates a batch job with the specified bucket list and delete object settings.
        CreateJobRequest request = new CreateJobRequest
        {
            ParentAsLocationName = locationName,
            JobId = jobId,
            Job = new Job
            {
                BucketList = bucketList,
                DeleteObject = new DeleteObject { PermanentObjectDeletionEnabled = true }
            },
            RequestId = jobId,
        };

        Operation<Job, OperationMetadata> response = storageBatchClient.CreateJob(request);
        string jobName = String.Empty;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Operation<Job, OperationMetadata> retrievedResponse = storageBatchClient.PollOnceCreateJob(response.Name);
            jobName = retrievedResponse.Metadata?.Job?.Name;

            if (!string.IsNullOrEmpty(jobName))
            {
                break;
            }
        }
        return jobName;
    }
}
