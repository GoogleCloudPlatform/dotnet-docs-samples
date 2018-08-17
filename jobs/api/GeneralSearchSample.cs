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
    /// The samples in this file introduce how to do a general search, including:
    ///
    /// - Basic keyword search
    ///
    /// - Filter on categories
    ///
    /// - Filter on employment types
    ///
    /// - Filter on date range
    ///
    /// - Filter on language codes
    ///
    /// - Filter on company display names
    ///
    /// - Filter on compensations
    /// </summary>
    public static class GeneralSearchSample
    {

        private static JobServiceService jobService = JobServiceUtils.JobService;

        //[START basic_keyword_search]

        /// <summary>
        /// Simple search jobs with keyword.
        /// </summary>
        public static void basicSearcJobs(string companyName, string query)
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

            // Perform a search for analyst  related jobs
            JobQuery jobQuery = new JobQuery
            {
                Query = query,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery, // Set the actual search term as defined in the jobQurey
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        //[END basic_keyword_search]

        // [START category_filter]

        /// <summary>
        /// Search on category filter.
        /// </summary>
        public static void CategoryFilterSearch(string companyName, IList<string> categories)
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
                Categories = categories,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery, // Set the actual search term as defined in the jobQurey
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END category_filter]

        // [START employment_types_filter]

        /// <summary>
        /// Search on employment types.
        /// </summary>
        public static void EmploymentTypesSearch(string companyName, IList<string> employmentTypes)
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
                EmploymentTypes = employmentTypes,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request =
                new SearchJobsRequest
                {
                    RequestMetadata = requestMetadata,
                    Query = jobQuery, // Set the actual search term as defined in the jobQuery
                    Mode = "JOB_SEARCH", // Set the search mode to a regular search
                };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END employment_types_filter]

        // [START date_range_filter]

        /// <summary>
        /// Search on date range.
        /// </summary>
        public static void DateRangeSearch(string companyName, string dateRange)
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

            JobQuery jobQuery = new JobQuery
            {
                PublishDateRange = dateRange,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery, // Set the actual search term as defined in the jobQuery
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END date_range_filter]

        // [START language_code_filter]

        /// <summary>
        /// Search on language codes.
        /// </summary>
        public static void LanguageCodeSearch(string companyName, IList<string> languageCodes)
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
                LanguageCodes = languageCodes,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery, // Set the actual search term as defined in the jobQurey
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END language_code_filter]

        // [START company_display_name_filter]

        /// <summary>
        /// Search on company display name.
        /// </summary>
        public static void CompanyDisplayNameSearch(string companyName, IList<string> companyDisplayNames)
        {
            // Make sure to set the requestMetadata the same as the associated search request
            RequestMetadata requestMetadata = new RequestMetadata
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionID",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery
            {
                CompanyDisplayNames = companyDisplayNames,
            };
            if (companyName != null)
            {
                jobQuery.CompanyNames = new[] { companyName };
            }

            SearchJobsRequest request = new SearchJobsRequest
            {
                RequestMetadata = requestMetadata,
                Query = jobQuery, // Set the actual search term as defined in the jobQurey
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END company_display_name_filter]

        // [START compensation_filter]

        /// <summary>
        /// Search on compensation.
        /// </summary>
        public static void CompensationSearch(string companyName)
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

            // Search jobs that pay between 10.50 and 15 USD per hour
            JobQuery jobQuery = new JobQuery
            {
                CompensationFilter = {
                    Type = "UNIT_AND_AMOUNT",
                    Units = {"HOURLY"},
                    Range = {
                        Max = {
                            CurrencyCode = "USD",
                            Units = 15L,
                        },
                        Min = {
                            CurrencyCode = "USD",
                            Units = 10L,
                            Nanos = 500000000,
                        },
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
                Query = jobQuery, // Set the actual search term as defined in the jobQurey
                Mode = "JOB_SEARCH", // Set the search mode to a regular search
            };

            SearchJobsResponse response = jobService.Jobs.Search(request).Execute();
            Console.WriteLine(response);
        }
        // [END compensation_filter]

        public static void Main(string[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            companyToBeCreated.DisplayName = "Google";
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = BasicJobSample.GenerateJobWithRequiredFields(companyName);
            jobToBeCreated.JobTitle = "Systems Administrator";
            jobToBeCreated.EmploymentTypes = new[] { "FULL_TIME" };
            jobToBeCreated.LanguageCode = "en-US";
            jobToBeCreated.CompensationInfo =
                    new CompensationInfo
                    {
                        Entries = new[] {
                            new CompensationEntry {
                                Type = "BASE",
                                Unit = "HOURLY",
                                Amount = {
                                    CurrencyCode = "USD",
                                    Units = 12L,
                                },
                            },
                        },
                    };
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            basicSearcJobs(companyName, "Systems Administrator");
            CategoryFilterSearch(companyName, new[] { "COMPUTER_AND_IT" });
            DateRangeSearch(companyName, "PAST_24_HOURS");
            EmploymentTypesSearch(companyName, new[] { "FULL_TIME", "CONTRACTOR", "PER_DIEM" });
            CompanyDisplayNameSearch(companyName, new[] { "Google" });
            CompensationSearch(companyName);
            LanguageCodeSearch(companyName, new[] { "pt-BR", "en-US" });

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}
