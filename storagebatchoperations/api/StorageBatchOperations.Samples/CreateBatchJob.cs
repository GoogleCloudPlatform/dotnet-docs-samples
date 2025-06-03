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
    private RewriteObject _rewriteObject;
    private PutMetadata _putMetadata;
    private DeleteObject _deleteObject;
    private PutObjectHold _putObjectHold;
    private Job _job;

    /// <summary>
    /// Creates a storage batch operation job.
    /// </summary>
    /// <param name="locationName">A resource name with pattern <c>projects/{project}/locations/{location}</c>.</param>
    /// <param name="bucketList">A bucket list contains list of buckets and their objects to be transformed.</param>
    /// <param name="jobId">It is id for the job and it should not be more than 128 characters and must include only
    /// characters available in DNS names, as defined by RFC-1123.</param>
    /// <param name="jobType">It is type of storage batch operation job.</param>
    public Job CreateBatchJob(LocationName locationName,
        BucketList bucketList,
        string jobId = "12345678910",
        string jobType = "DeleteObject")
    {
        StorageBatchOperationsClient storageBatchOperationsClient = StorageBatchOperationsClient.Create();

        if (jobType == "RewriteObject")
        {
            _rewriteObject = new RewriteObject { KmsKey = "projects/{project}/locations/{location}/keyRings/{keyring}/cryptoKeys/{key}" };

            _job = new Job
            {
                RewriteObject = _rewriteObject,
                BucketList = bucketList
            };
        }
        else if (jobType == "PutMetadata")
        {
            _putMetadata = new PutMetadata { CacheControl = "", ContentDisposition = "", ContentEncoding = "", ContentLanguage = "", ContentType = "", CustomTime = "" };

            _job = new Job
            {
                PutMetadata = _putMetadata,
                BucketList = bucketList
            };
        }
        else if (jobType == "DeleteObject")
        {
            _deleteObject = new DeleteObject { PermanentObjectDeletionEnabled = true };

            _job = new Job
            {
                DeleteObject = _deleteObject,
                BucketList = bucketList
            };
        }
        else if (jobType == "PutObjectHoldEventBasedHoldSet")
        {
            _putObjectHold = new PutObjectHold { EventBasedHold = PutObjectHold.Types.HoldStatus.Set };

            _job = new Job
            {
                PutObjectHold = _putObjectHold,
                BucketList = bucketList
            };
        }
        else if (jobType == "PutObjectHoldEventBasedHoldUnSet")
        {
            _putObjectHold = new PutObjectHold { EventBasedHold = PutObjectHold.Types.HoldStatus.Unset };

            _job = new Job
            {
                PutObjectHold = _putObjectHold,
                BucketList = bucketList
            };
        }
        else if (jobType == "PutObjectHoldTemporaryHoldSet")
        {
            _putObjectHold = new PutObjectHold { TemporaryHold = PutObjectHold.Types.HoldStatus.Set };

            _job = new Job
            {
                PutObjectHold = _putObjectHold,
                BucketList = bucketList
            };
        }
        else if (jobType == "PutObjectHoldTemporaryHoldUnSet")
        {
            _putObjectHold = new PutObjectHold { TemporaryHold = PutObjectHold.Types.HoldStatus.Unset };

            _job = new Job
            {
                PutObjectHold = _putObjectHold,
                BucketList = bucketList
            };
        }

        CreateJobRequest request = new CreateJobRequest
        {
            ParentAsLocationName = locationName,
            JobId = jobId,
            Job = _job,
            RequestId = jobId,
        };

        Operation<Job, OperationMetadata> response = storageBatchOperationsClient.CreateJob(request);
        Operation<Job, OperationMetadata> completedResponse = response.PollUntilCompleted();

        Job result = completedResponse.Result;
        string operationName = response.Name;
        Operation<Job, OperationMetadata> retrievedResponse = storageBatchOperationsClient.PollOnceCreateJob(operationName);

        if (retrievedResponse.IsCompleted)
        {
            Job retrievedResult = retrievedResponse.Result;
            return retrievedResult;
        }
        Console.WriteLine($"The Storage Batch Operation Job (Name: {result.Name}) is created");
        return result;
    }
}
// [END storage_batch_create_job]
