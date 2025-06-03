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

// [START storage_batch_get_job]

using Google.Cloud.StorageBatchOperations.V1;
using System;

public class GetBatchJobSample
{
    /// <summary>
    /// Gets a storage batch operation job.
    /// </summary>
    /// <param name="jobName">The name of the job to get. Format: projects/{project_id}/locations/{location_id}/jobs/{job_id}.</param>
    public Job GetBatchJob(string jobName = "projects/{project_id}/locations/{location_id}/jobs/{job_id}")
    {
        StorageBatchOperationsClient storageBatchOperationsClient = StorageBatchOperationsClient.Create();
        GetJobRequest request = new GetJobRequest
        {
            Name = jobName
        };
        var response = storageBatchOperationsClient.GetJob(request);
        Console.WriteLine($"The Name of Storage Batch Operation Job is : {response.Name}");
        return response;
    }
}
// [END storage_batch_get_job]
