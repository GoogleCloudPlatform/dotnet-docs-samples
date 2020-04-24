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

using static Google.Cloud.Talent.V4Beta1.Job.Types;

using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class CreateJobSample
    {
        // [START job_search_create_job_beta]
        public static object CreateJob(string projectId, string tenantId, string companyId, string requisitionId, string jobApplicationUrl)
        {
            JobServiceClient jobServiceClient = JobServiceClient.Create();
            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);

            ApplicationInfo applicationInfo = new ApplicationInfo();
            applicationInfo.Uris.Add(jobApplicationUrl);

            Job job = new Job
            {
                Company = companyId,
                RequisitionId = requisitionId,
                Title = "Software Developer",
                Description = "Develop, maintain the software solutions.",
                ApplicationInfo = applicationInfo,
                LanguageCode = "en-US"
            };
            string[] addresses = { "1600 Amphitheatre Parkway, Mountain View, CA 94043", "111 8th Avenue, New York, NY 10011" };
            job.Addresses.Add(addresses);

            CreateJobRequest request = new CreateJobRequest
            {
                ParentAsTenantName = tenantName,
                Job = job
            };

            Job response = jobServiceClient.CreateJob(request);

            Console.WriteLine($"Created Job: {response.Name}");
            return 0;
        }
        // [END job_search_create_job_beta]
    }
}
