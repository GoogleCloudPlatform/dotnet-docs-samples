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
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

[Collection(nameof(StorageFixture))]
public class CancelBatchJobTest
{
    private readonly StorageFixture _fixture;
    private readonly BucketList.Types.Bucket _bucket = new();
    private readonly BucketList _bucketList = new();
    private readonly PrefixList _prefixListObject = new();

    public CancelBatchJobTest(StorageFixture fixture)
    {
        int i = 10;
        _fixture = fixture;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: false, registerForDeletion: true);
        while (i >= 0)
        {
            var objectName = _fixture.GenerateName();
            var objectContent = _fixture.GenerateContent();
            byte[] byteObjectContent = Encoding.UTF8.GetBytes(objectContent);
            MemoryStream streamObjectContent = new MemoryStream(byteObjectContent);
            _fixture.Client.UploadObject(bucketName, objectName, "application/text", streamObjectContent);
            i--;
        }
        _bucket = new BucketList.Types.Bucket
        {
            Bucket_ = bucketName,
            PrefixList = _prefixListObject
        };
        _bucketList.Buckets.Insert(0, _bucket);
    }

    [Fact]
    public void CancelBatchJob()
    {
        CancelBatchJobSample cancelBatchJob = new CancelBatchJobSample();
        ListBatchJobsSample listBatchJobs = new ListBatchJobsSample();
        GetBatchJobSample getBatchJob = new GetBatchJobSample();
        string filter = "state:canceled";
        int pageSize = 10;
        string orderBy = "create_time";
        var jobId = _fixture.GenerateJobId();
        var createdJob = CreateBatchJob(_fixture.LocationName, _bucketList, jobId);
        var jobResponse = cancelBatchJob.CancelBatchJob(createdJob);
        PollUntilCancelled();
        var batchJobs = listBatchJobs.ListBatchJobs(_fixture.LocationName, filter, pageSize, orderBy);
        Assert.Contains(batchJobs, job => job.Name == createdJob);
        Job cancelledJob = getBatchJob.GetBatchJob(createdJob);
        Assert.Equal("Canceled", cancelledJob.State.ToString());
        _fixture.DeleteBatchJob(createdJob);
    }

    public static string CreateBatchJob(LocationName locationName,
        BucketList bucketList,
        string jobId = "12345678910")
    {
        StorageBatchOperationsClient storageBatchOperationsClient = StorageBatchOperationsClient.Create();

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

        Operation<Job, OperationMetadata> response = storageBatchOperationsClient.CreateJob(request);

        string operationName = response.Name;
        Operation<Job, OperationMetadata> retrievedResponse = storageBatchOperationsClient.PollOnceCreateJob(operationName);

        if (retrievedResponse.IsCompleted)
        {
            string jobName = retrievedResponse.Metadata.Job.Name;
            return jobName;
        }
        else
        {
            retrievedResponse = retrievedResponse.PollOnce();
            string jobName = retrievedResponse.Metadata.Job.Name;
            return jobName;
        }
    }

    /// <summary>
    /// Wait for 15 seconds until completion of cancel batch job operation.
    /// </summary>
    private static void PollUntilCancelled() => Thread.Sleep(15000);
}
