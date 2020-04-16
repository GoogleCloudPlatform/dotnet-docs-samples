// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax;
using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class ListJobsSample
    {
        // [START job_search_list_jobs]
        public static object ListJobs(string projectId, string tenantId, string filter)
        {
            JobServiceClient jobServiceClient = JobServiceClient.Create();

            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);
            ListJobsRequest request = new ListJobsRequest
            {
                ParentAsTenantName = tenantName,
                Filter = filter
            };
            PagedEnumerable<ListJobsResponse, Job> jobs = jobServiceClient.ListJobs(request);

            foreach (var job in jobs)
            {
                Console.WriteLine($"Job name: {job.Name}");
                Console.WriteLine($"Job requisition ID: {job.RequisitionId}");
                Console.WriteLine($"Job title: {job.Title}");
                Console.WriteLine($"Job description: {job.Description}");
            }
            return 0;
        }
        // [END job_search_list_jobs]
    }
}
