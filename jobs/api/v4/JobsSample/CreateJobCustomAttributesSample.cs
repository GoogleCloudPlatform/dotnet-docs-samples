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
    internal class CreateJobCustomAttributesSample
    {
        // [START job_search_create_job_custom_attributes]
        public static object CreateJobCustomAttributes(string projectId, string tenantId, string companyId, string requisitionId)
        {
            JobServiceClient jobServiceClient = JobServiceClient.Create();
            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);

            // Custom attribute can be string or numeric value, and can be filtered in search queries.
            // https://cloud.google.com/talent-solution/job-search/docs/custom-attributes
            CustomAttribute customAttributes = new CustomAttribute
            {
                Filterable = true
            };
            customAttributes.StringValues.Add("Internship");
            customAttributes.StringValues.Add("Intern");
            customAttributes.StringValues.Add("Apprenticeship");

            Job job = new Job
            {
                Company = companyId,
                RequisitionId = requisitionId,
                Title = "Software Developer I",
                Description = "This is a description of this <i>wonderful</i> job!",
                LanguageCode = "en-US"
            };
            job.CustomAttributes.Add("FOR_STUDENTS", customAttributes);

            CreateJobRequest request = new CreateJobRequest
            {
                ParentAsTenantName = tenantName,
                Job = job
            };

            Job response = jobServiceClient.CreateJob(request);

            Console.WriteLine($"Created Job: {response.Name}");
            return 0;
        }
        // [END job_search_create_job_custom_attributes]
    }
}
