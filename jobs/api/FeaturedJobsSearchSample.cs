/*
 * Copyright 2018 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;

namespace Google.Samples
{

    /// <summary>
    /// The sample in this file introduce featured job, including:
    ///
    /// - Construct a featured job
    ///
    /// - Search featured job
    /// </summary>
    public static class FeaturedJobsSearchSample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START featured_job]

        /// <summary>
        /// Creates a job as featured.
        /// </summary>
        public static Job GenerateFeaturedJob(string companyName)
        {
            // requisition id should be a unique Id in your system.
            String requisitionId =
                "featuredJob:" + new Random().Next();

            Job job = new Job
            {
                RequisitionId = requisitionId,
                JobTitle = "Software Engineer",
                ApplicationUrls = { "http://careers.google.com" },
                Description =
                        "Design, develop, test, deploy, maintain and improve software.",
                // Featured job is the job with positive promotion value
                PromotionValue = 2,
            };
            Console.WriteLine("Job generated: " + job);
            return job;
        }
        // [END featured_job]

        // [START search_featured_job]

        /// <summary>
        /// Searches featured jobs.
        /// </summary>
        public static void SearchFeaturedJobs(string companyName)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };

            JobQuery jobQuery = new JobQuery
            {
                Query = "Software Engineer",
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                // Set the search mode to a featured search,
                // which would only search the jobs with positive promotion value.
                Mode = "FEATURED_JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END search_featured_job]

        public static void Main(String[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = GenerateFeaturedJob(companyName);
            String jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            SearchFeaturedJobs(companyName);

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}
