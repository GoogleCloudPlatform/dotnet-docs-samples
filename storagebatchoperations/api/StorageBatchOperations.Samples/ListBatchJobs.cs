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

// [START storage_batch_list_jobs]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.StorageBatchOperations.V1;
using System;
using System.Collections.Generic;

public class ListBatchJobsSample
{
    /// <summary>
    /// Lists storage batch operation jobs.
    /// </summary>
    /// <param name="locationName">A resource name with pattern <c>projects/{project}/locations/{location}</c>.</param>
    /// <param name="filter">The field to filter the list of storage batch operation jobs.</param>
    /// <param name="pageSize">The page size to retrieve page of known size.</param>
    /// <param name="orderBy">The field to sort the list of storage batch operation jobs. Supported fields are name and create_time.</param>
    public IEnumerable<Job> ListBatchJobs(LocationName locationName,
        string filter = "state:failed",
        int pageSize = 100,
        string orderBy = "name")
    {
        StorageBatchOperationsClient operationsClient = StorageBatchOperationsClient.Create();
        // Create a request to list the batch jobs.
        ListJobsRequest request = new ListJobsRequest
        {
            ParentAsLocationName = locationName,
            Filter = filter,
            OrderBy = orderBy
        };

        PagedEnumerable<ListJobsResponse, Job> response = operationsClient.ListJobs(request);
        Console.WriteLine("Storage Batch Operation Jobs are as follows:");
        foreach (var item in response)
        {
            Console.WriteLine(item);
        }
        // Retrieve a single page of known size
        Page<Job> singlePage = response.ReadPage(pageSize);
        Console.WriteLine($"A single page of {pageSize} page size of Storage Batch Operation Jobs are as follows:");
        foreach (Job item in singlePage)
        {
            Console.WriteLine(item);
        }
        return response;
    }
}
// [END storage_batch_list_jobs]
