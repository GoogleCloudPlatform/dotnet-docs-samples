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
    /// This file contains the samples about CustomAttribute, including:
    ///
    /// - Construct a Job with CustomAttribute
    ///
    /// - Search Job with CustomAttributeFilter
    /// </summary>
    public static class CustomAttributeSample
    {

        private static readonly JobServiceService jobService = JobServiceUtils.JobService;

        // [START custom_attribute_job]

        /// <summary>
        /// Generate a job with a custom attribute.
        /// </summary>
        public static Job generateJobWithACustomAttribute(string companyName)
        {
            // requisition id should be a unique Id in your system.
            string requisitionId = "jobWithACustomAttribute:" + new Random().Next();

            // Constructs custom attributes map
            IDictionary<string, CustomAttribute> customAttributes = new Dictionary<string, CustomAttribute>()
            {
                ["someFieldName1"] = new CustomAttribute
                {
                    StringValues = new StringValues
                    {
                        Values = { "value1" }
                    },
                    Filterable = true,
                },
                ["someFieldName2"] = new CustomAttribute
                {
                    LongValue = 256L,
                    Filterable = true,
                }
            };

            // Creates job with custom attributes
            Job job = new Job
            {
                CompanyName = companyName,
                RequisitionId = requisitionId,
                JobTitle = "Software Engineer",
                ApplicationUrls = { "http://careers.google.com" },
                Description =
                        "Design, develop, test, deploy, maintain and improve software.",
                CustomAttributes = customAttributes,
            };
            Console.WriteLine("Job generated: " + job);
            return job;
        }
        // [END custom_attribute_job]

        // [START custom_attribute_filter_string_value]

        /// <summary>
        /// CustomAttributeFilter on string value CustomAttribute
        /// </summary>
        public static void FiltersOnstringValueCustomAttribute()
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

            string customAttributeFilter = "NOT EMPTY(someFieldName1)";
            JobQuery jobQuery = new JobQuery
            {
                CustomAttributeFilter = customAttributeFilter,
            };

            SearchJobsRequest searchJobsRequest =
                new SearchJobsRequest
                {
                    Query = jobQuery,
                    RequestMetadata = requestMetadata,
                    JobView = "FULL",
                };
            SearchJobsResponse response = jobService.Jobs.Search(searchJobsRequest).Execute();
            Console.WriteLine(response);
        }
        // [END custom_attribute_filter_string_value]

        // [START custom_attribute_filter_long_value]

        /// <summary>
        /// CustomAttributeFilter on Long value CustomAttribute
        /// </summary>
        public static void FiltersOnLongValueCustomAttribute()
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

            string customAttributeFilter = "(255 <= someFieldName2) AND (someFieldName2 <= 257)";
            JobQuery jobQuery = new JobQuery
            {
                CustomAttributeFilter = customAttributeFilter,
            };

            SearchJobsRequest searchJobsRequest =
                new SearchJobsRequest
                {
                    Query = jobQuery,
                    RequestMetadata = requestMetadata,
                    JobView = "FULL",
                };
            SearchJobsResponse response = jobService.Jobs.Search(searchJobsRequest).Execute();
            Console.WriteLine(response);
        }
        // [END custom_attribute_filter_long_value]

        // [START custom_attribute_filter_multi_attributes]

        /// <summary>
        /// CustomAttributeFilter on multiple CustomAttributes
        /// </summary>
        public static void FiltersOnMultiCustomAttributes()
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

            string customAttributeFilter = "(someFieldName1 = \"value1\") "
                + "AND ((255 <= someFieldName2) OR (someFieldName2 <= 213))";
            JobQuery jobQuery = new JobQuery
            {
                CustomAttributeFilter = customAttributeFilter,
            };

            SearchJobsRequest searchJobsRequest =
                new SearchJobsRequest
                {
                    Query = jobQuery,
                    RequestMetadata = requestMetadata,
                    JobView = "FULL",
                };
            SearchJobsResponse response = jobService.Jobs.Search(searchJobsRequest).Execute();
            Console.WriteLine(response);
        }
        // [END custom_attribute_filter_multi_attributes]

        public static void Main(string[] args)
        {
            Company companyToBeCreated = BasicCompanySample.GenerateCompany();
            string companyName = BasicCompanySample.CreateCompany(companyToBeCreated).Name;

            Job jobToBeCreated = generateJobWithACustomAttribute(companyName);
            string jobName = BasicJobSample.CreateJob(jobToBeCreated).Name;

            // Wait several seconds for post processing
            Thread.Sleep(10000);
            FiltersOnstringValueCustomAttribute();
            FiltersOnLongValueCustomAttribute();
            FiltersOnMultiCustomAttributes();

            BasicJobSample.DeleteJob(jobName);
            BasicCompanySample.DeleteCompany(companyName);
        }
    }
}