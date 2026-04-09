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

// [START storage_batch_create_job]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageBatchOperations.V1;
using Google.LongRunning;
using System;

public class CreateBatchJobSample
{
    /// <summary>
    /// Creates a storage batch operation job.
    /// </summary>
    /// <param name="locationName">A resource name with pattern <c>projects/{project}/locations/{location}</c>.</param>
    /// <param name="bucketList">A bucket list contains list of buckets and their objects to be transformed.</param>
    /// <param name="jobId">It is id for the job and it should not be more than 128 characters and must include only
    /// characters available in DNS names, as defined by RFC-1123.</param>
    /// <param name="jobType">It is type of storage batch operation job.</param>
    /// <param name="jobTransformationObject">It is object of type either RewriteObject or PutMetadata.</param>
    public Job CreateBatchJob(LocationName locationName,
        BucketList bucketList,
        string jobId = "12345678910",
        string jobType = "DeleteObject",
        object jobTransformationObject = null)
    {
        StorageBatchOperationsClient operationsClient = StorageBatchOperationsClient.Create();
        Job job = null;
        PutObjectHold putObjectHold;
        switch (jobType)
        {
            case "RewriteObject":
                job = new Job
                {
                    RewriteObject = (RewriteObject) jobTransformationObject,
                    BucketList = bucketList
                };
                break;
            case "PutMetadata":
                job = new Job
                {
                    PutMetadata = (PutMetadata) jobTransformationObject,
                    BucketList = bucketList
                };
                break;
            case "DeleteObject":
                var deleteObject = new DeleteObject { PermanentObjectDeletionEnabled = true };

                job = new Job
                {
                    DeleteObject = deleteObject,
                    BucketList = bucketList
                };
                break;
            case "PutObjectHoldEventBasedHoldSet":
                putObjectHold = new PutObjectHold { EventBasedHold = PutObjectHold.Types.HoldStatus.Set };

                job = new Job
                {
                    PutObjectHold = putObjectHold,
                    BucketList = bucketList
                };
                break;
            case "PutObjectHoldEventBasedHoldUnSet":
                putObjectHold = new PutObjectHold { EventBasedHold = PutObjectHold.Types.HoldStatus.Unset };

                job = new Job
                {
                    PutObjectHold = putObjectHold,
                    BucketList = bucketList
                };
                break;
            case "PutObjectHoldTemporaryHoldSet":
                putObjectHold = new PutObjectHold { TemporaryHold = PutObjectHold.Types.HoldStatus.Set };

                job = new Job
                {
                    PutObjectHold = putObjectHold,
                    BucketList = bucketList
                };
                break;
            case "PutObjectHoldTemporaryHoldUnSet":
                putObjectHold = new PutObjectHold { TemporaryHold = PutObjectHold.Types.HoldStatus.Unset };

                job = new Job
                {
                    PutObjectHold = putObjectHold,
                    BucketList = bucketList
                };
                break;
        }

        // Create a job request with the specified location, job ID, and job details.
        CreateJobRequest request = new CreateJobRequest
        {
            ParentAsLocationName = locationName,
            JobId = jobId,
            Job = job,
            RequestId = jobId,
        };

        Operation<Job, OperationMetadata> response = operationsClient.CreateJob(request);
        Operation<Job, OperationMetadata> completedResponse = response.PollUntilCompleted();
        Job result = completedResponse.Result;
        Console.WriteLine($"The Storage Batch Operation Job (Name: {result.Name}) is created");
        return result;
    }
}
// [END storage_batch_create_job]
