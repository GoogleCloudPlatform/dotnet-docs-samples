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

using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class GetJobSample
    {
        // [START job_search_get_job_beta]
        public static object GetJob(string projectId, string tenantId, string jobId)
        {
            JobServiceClient jobServiceClient = JobServiceClient.Create();
            JobName jobName = JobName.FromProjectTenantJob(projectId, tenantId, jobId);
            GetJobRequest request = new GetJobRequest
            {
                JobName = jobName
            };
            var response = jobServiceClient.GetJob(request);

            Console.WriteLine($"Job name: {response.Name}");
            Console.WriteLine($"Requisition ID: {response.RequisitionId}");
            Console.WriteLine($"Title: {response.Title}");
            Console.WriteLine($"Description: {response.Description}");
            Console.WriteLine($"Posting language: {response.LanguageCode}");
            foreach (string address in response.Addresses)
            {
                Console.WriteLine($"Address: {address}");
            }
            foreach (string email in response.ApplicationInfo.Emails)
            {
                Console.WriteLine($"Email: {email}");
            }
            foreach (string websiteUri in response.ApplicationInfo.Uris)
            {
                Console.WriteLine($"Website: {websiteUri}");
            }
            return 0;
        }
        // [END job_search_get_job_beta]
    }
}
