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

// [START storage_batch_delete_job]

using Google.Cloud.StorageBatchOperations.V1;
using System;

public class DeleteBatchJobSample
{
    /// <summary>
    /// Deletes a storage batch operation job.
    /// </summary>
    /// <param name="jobName">The name of the job to delete. Format: projects/{project_id}/locations/{location_id}/jobs/{job_id}.</param>
    public void DeleteBatchJob(string jobName = "projects/{project_id}/locations/{location_id}/jobs/{job_id}")
    {
        StorageBatchOperationsClient operationsClient = StorageBatchOperationsClient.Create();
        // Create a request to delete the job.
        DeleteJobRequest request = new DeleteJobRequest
        {
            Name = jobName
        };
        // Delete the job.
        operationsClient.DeleteJob(request);
        Console.WriteLine($"The Storage Batch Operation Job (Name : {jobName}) is deleted");
    }
}
// [END storage_batch_delete_job]
