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
    /// The samples in this file introduce how to do a search with location filter, including:
    ///
    /// - Basic search with location filter
    ///
    /// - Keyword search with location filter
    ///
    /// - Location filter on city level
    ///
    /// - Broadening search with location filter
    ///
    /// - Location filter of multiple locations
    /// </summary>
    public static class LocationSearchSample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        // [START basic_location_search]

        /// <summary>
        /// Basic location Search
        /// </summary>
        public static void BasicLocationSearch(string companyName, string location, double distance)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash the userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };
            LocationFilter locationFilter = new LocationFilter
            {
                Name = location,
                DistanceInMiles = distance,
            };
            JobQuery jobQuery = new JobQuery
            {
                LocationFilters = { locationFilter },
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                Mode = "JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END basic_location_search]

        // [START keyword_location_search]

        /// <summary>
        /// Keyword location Search
        /// </summary>
        public static void KeywordLocationSearch(string companyName, string location, double distance,
            string keyword)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash the userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };
            LocationFilter locationFilter = new LocationFilter
            {
                Name = location,
                DistanceInMiles = distance,
            };
            JobQuery jobQuery = new JobQuery
            {
                Query = keyword,
                LocationFilters = { locationFilter },
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                Mode = "JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END keyword_location_search]

        // [START city_location_search]

        /// <summary>
        /// City location Search
        /// </summary>
        public static void CityLocationSearch(string companyName, string location)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash the userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };
            LocationFilter locationFilter = new LocationFilter
            {
                Name = location,
            };
            JobQuery jobQuery = new JobQuery
            {
                LocationFilters = { locationFilter },
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                Mode = "JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END city_location_search]

        // [START multi_locations_search]

        /// <summary>
        /// Multiple locations Search
        /// </summary>
        public static void MultiLocationsSearch(string companyName, string location1, double distance1,
            string location2)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash the userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };
            JobQuery jobQuery = new JobQuery
            {
                LocationFilters = {
                            new LocationFilter {
                                Name = location1,
                                DistanceInMiles = distance1,
                            },
                            new LocationFilter {
                                Name = location2,
                            },
                    },
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                Mode = "JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END multi_locations_search]

        // [START broadening_location_search]

        /// <summary>
        /// Broadening location Search
        /// </summary>
        public static void BroadeningLocationsSearch(string companyName, string location)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash the userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com",
            };
            JobQuery jobQuery = new JobQuery
            {
                LocationFilters = {
                        new LocationFilter{
                            Name = location,
                        },
                    },
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }
            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery,
                EnableBroadening = true,
                Mode = "JOB_SEARCH",
            };
            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END broadening_location_search]

        public static void Main(string[] args)
        {
            string location = args.Length >= 1 ? args[0] : "Mountain View, CA";
            double distance = args.Length >= 2 ? Double.Parse(args[1]) : 0.5;
            string keyword = args.Length >= 3 ? args[2] : "Software Engineer";
            string location2 = args.Length >= 4 ? args[3] : "Sunnyvale, CA";

            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = BasicJobSample.GenerateJobWithRequiredFields(companyName);
            jobToBeCreated.Locations = new[] { location };
            jobToBeCreated.JobTitle = keyword;
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;
            Job jobToBeCreated2 = BasicJobSample.GenerateJobWithRequiredFields(companyName);
            jobToBeCreated2.Locations = new[] { location2 };
            jobToBeCreated2.JobTitle = keyword;
            string jobName2 = BasicJobSample.CreateJob(jobToBeCreated2).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            BasicLocationSearch(companyName, location, distance);
            CityLocationSearch(companyName, location);
            BroadeningLocationsSearch(companyName, location);
            KeywordLocationSearch(companyName, location, distance, keyword);
            MultiLocationsSearch(companyName, location, distance, location2);

            BasicJobSample.DeleteJob(jobName);
            BasicJobSample.DeleteJob(jobName2);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}
