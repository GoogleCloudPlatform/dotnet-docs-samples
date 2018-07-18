
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
using System.Collections.Generic;
using System.Threading;
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;

namespace Google.Samples
{

    /// <summary>
    /// The samples in this file introduce how to do a commute search.
    ///
    /// Note: Commute Search is different from location search. It only take latitude and longitude as
    /// the start location.
    /// </summary>
    public static class CommuteSearchSample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START commute_search]

        public static void CommuteSearch(string companyName)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata =
                new RequestMetadata
                {
                    // Make sure to hash your userID
                    UserId = "HashedUserId",
                    // Make sure to hash the sessionID
                    SessionId = "HashedSessionID",
                    // Domain of the website where the search is conducted
                    Domain = "www.google.com",
                };
            JobQuery jobQuery =
                new JobQuery
                {
                    CommuteFilter =
                        {
                            RoadTraffic = "TRAFFIC_FREE",
                            Method = "TRANSIT",
                            TravelTime = "1000s",
                            StartLocation =
                                {
                                    Latitude = 37.422408,
                                    Longitude = -122.085609
                                }
                        }
                };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string> { companyName };
            }
            SearchJobsRequest searchJobsRequest =
                new SearchJobsRequest
                {
                    Query = jobQuery,
                    RequestMetadata = requestMetadata,
                    JobView = "FULL",
                    EnablePreciseResultSize = true,
                };
            SearchJobsResponse response = jobService.Jobs.Search(searchJobsRequest).Execute();
            Console.WriteLine(response);
        }
        // [END commute_search]

        public static void Main(string[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = BasicJobSample.GenerateJobWithRequiredFields(companyName);
            jobToBeCreated.Locations = new List<string> { "1600 Amphitheatre Pkwy, Mountain View, CA 94043" };
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            CommuteSearch(companyName);

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}