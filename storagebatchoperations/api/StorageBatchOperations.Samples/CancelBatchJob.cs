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

// [START storage_batch_cancel_job]

using Google.Cloud.StorageBatchOperations.V1;
using System;

public class CancelBatchJobSample
{
    /// <summary>
    /// Cancels a storage batch operation job.
    /// </summary>
    /// <param name="jobName">The name of the job to cancel. Format: projects/{project_id}/locations/{location_id}/jobs/{job_id}.</param>
    public CancelJobResponse CancelBatchJob(string jobName = "projects/{project_id}/locations/{location_id}/jobs/{job_id}")
    {
        StorageBatchOperationsClient storageBatchOperationsClient = StorageBatchOperationsClient.Create();
        CancelJobRequest request = new CancelJobRequest
        {
            Name = jobName,
            RequestId = jobName
        };
        var response = storageBatchOperationsClient.CancelJob(request);
        Console.WriteLine($"The Storage Batch Operation Job (Name: {jobName}) is cancelled");
        return response;
    }
}
// [END storage_batch_cancel_job]
