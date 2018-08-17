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
    /// The sample in this file introduce how to do a histogram search.
    /// </summary>
    public static class HistogramSample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START histogram_search]

        /// <summary>
        /// Histogram search
        /// </summary>
        public static void HistogramSearch(string companyName)
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

            HistogramFacets histogramFacets = new HistogramFacets
            {
                SimpleHistogramFacets = { "COMPANY_ID" },
                CustomAttributeHistogramFacets = {
                    new CustomAttributeHistogramRequest {
                        Key = "someFieldName",
                        StringValueHistogram = true,
                    },
                },
            };
            // conducted.
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Mode = "JOB_SEARCH",
                HistogramFacets = histogramFacets,
            };
            if (companyName != null)
            {
                request.Query = new JobQuery
                {
                    CompanyNames = { companyName },
                };
            }

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END histogram_search]

        public static void main(string[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = CustomAttributeSample.generateJobWithACustomAttribute(companyName);
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            HistogramSearch(companyName);

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}
