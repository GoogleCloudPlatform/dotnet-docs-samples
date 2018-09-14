/*
 * Copyright (c) 2018 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudTalentSolution.v3;
using Google.Apis.CloudTalentSolution.v3.Data;
using Google.Apis.Services;

namespace GoogleCloudSamples
{
    public class CustomAttributes
    {
        public static GoogleCredential credential;
        public static CloudTalentSolutionService jobServiceClient;
        public static string projectId;
        public static string parent;
        public static Company myCompany;

        public static void Initialize()
        {
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    Google.Apis.CloudTalentSolution.v3.CloudTalentSolutionService.Scope.Jobs
                });
            }
            // Instantiate the Cloud Key Management Service API.
            jobServiceClient = new CloudTalentSolutionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            parent = $"projects/{projectId}";
        }

        public static void Main(string[] args)
        {
            Initialize();
            SetupCompany();

            // Test create job

            Job customJob = GenerateJobWithCustomAttribute(myCompany.Name);

            customJob = CreateJob(customJob);

            FiltersOnStringValueCustomAttribute();
            FiltersOnLongValueCustomAttribute();
            FiltersOnMultiCustomAttributes();

            DeleteJob(customJob.Name);
            DeleteCompany(myCompany.Name);
        }

        // [START custom_attribute_job]

        public static Job GenerateJobWithCustomAttribute(string companyName)
        {
            ApplicationInfo applicationInfo = new ApplicationInfo()
            {
                Uris = new List<string>
                {
                    "http://careers.google.com"
                }
            };

            CustomAttribute customAttribute = new CustomAttribute();
            customAttribute.StringValues = new List<String>()
            {
                "attribute"
            };

            Dictionary<String, CustomAttribute> customAttributes = new Dictionary<string, CustomAttribute>
            {
                { "custom_attribute", customAttribute }
            };

            Job customJob = new Job()
            {
                Title = "Software Engineer",
                RequisitionId = generateRandom(),
                Description = "My Job Description Here",
                CompanyName = myCompany.Name,
                ApplicationInfo = applicationInfo,
                CustomAttributes = customAttributes
            };

            Console.WriteLine("Created custom job: " + customJob.Title);
            return customJob;
        }
        // [END custom_attribute_job]

        // [START custom_attribute_filter_string_value]

        public static void FiltersOnStringValueCustomAttribute()
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                UserId = "HashedUserId",
                SessionId = "HashedSessionId",
                Domain = "www.google.com"
            };

            string customAttributeFilter = "NOT EMPTY(custom_attribute)";
            JobQuery jobQuery = new JobQuery()
            {
                CustomAttributeFilter = customAttributeFilter
            };

            SearchJobsRequest searchJobsRequest = new SearchJobsRequest()
            {
                JobQuery = jobQuery,
                RequestMetadata = requestMetadata,
                JobView = "JOB_VIEW_FULL"
            };

            SearchJobsResponse response = jobServiceClient.Projects.Jobs.Search(searchJobsRequest, parent).Execute();

            Console.WriteLine("Searched on custom attribute: " + ToJsonString(response));
        }
        // [END custom_attribute_filter_string_value]

        // [START custom_attribute_filter_long_value]

        public static void FiltersOnLongValueCustomAttribute()
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                UserId = "HashedUserId",
                SessionId = "HashedSessionId",
                Domain = "www.google.com"
            };

            string customAttributeFilter = "(255 <= someFieldName2) AND (someFieldName2 <= 267)";
            JobQuery jobQuery = new JobQuery()
            {
                CustomAttributeFilter = customAttributeFilter
            };

            SearchJobsRequest searchJobsRequest = new SearchJobsRequest()
            {
                JobQuery = jobQuery,
                RequestMetadata = requestMetadata,
                JobView = "JOB_VIEW_FULL"
            };

            SearchJobsResponse response = jobServiceClient.Projects.Jobs.Search(searchJobsRequest, parent).Execute();

            Console.WriteLine("Searched on custom long attribute: " + ToJsonString(response));
        }
        // [END custom_attribute_filter_long_value]

        // [START custom_attribute_filter_multi_attributes]

        public static void FiltersOnMultiCustomAttributes()
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                UserId = "HashedUserId",
                SessionId = "HashedSessionId",
                Domain = "www.google.com"
            };

            string customAttributeFilter = "(someFiledName1 = \"value1\") " +
                "AND ((255 <= someFieldName2) OR (someFieldName2 <= 213))";
            JobQuery jobQuery = new JobQuery()
            {
                CustomAttributeFilter = customAttributeFilter
            };

            SearchJobsRequest searchJobsRequest = new SearchJobsRequest()
            {
                JobQuery = jobQuery,
                RequestMetadata = requestMetadata,
                JobView = "JOB_VIEW_FULL"
            };

            SearchJobsResponse response = jobServiceClient.Projects.Jobs.Search(searchJobsRequest, parent).Execute();

            Console.WriteLine("Searched on cross-field-filtering: " + ToJsonString(response));
        }
        // [END custom_attribute_filter_multi_attributes]

        public static Job UpdateJob(string jobName, Job toBeUpdated)
        {
            UpdateJobRequest updateJobRequest = new UpdateJobRequest()
            {
                Job = toBeUpdated
            };
            Job jobUpdated = jobServiceClient.Projects.Jobs.Patch(updateJobRequest, jobName).Execute();
            Console.WriteLine("Job updated: " + ToJsonString(jobUpdated));
            return jobUpdated;
        }

        public static Job UpdateJobWithFieldMask(string jobName, string fieldMask, Job jobToBeUpdated)
        {
            UpdateJobRequest updateJobRequest = new UpdateJobRequest()
            {
                Job = jobToBeUpdated,
                UpdateMask = fieldMask
            };

            Job jobUpdated = jobServiceClient.Projects.Jobs.Patch(updateJobRequest, jobName).Execute();
            Console.WriteLine("Job updated with fieldMask: " + ToJsonString(jobUpdated));
            return jobUpdated;
        }

        public static Job CreateJob(Job jobToBeCreated)
        {
            CreateJobRequest createJobRequest = new CreateJobRequest();
            createJobRequest.Job = jobToBeCreated;

            Job jobCreated = jobServiceClient.Projects.Jobs.Create(createJobRequest, parent).Execute();
            Console.WriteLine("Job created: " + jobCreated.Name);
            return jobCreated;
        }


        public static Job getJob(String jobName)
        {
            Job jobExisted = jobServiceClient.Projects.Jobs.Get(jobName).Execute();
            Console.WriteLine("Job exists: " + jobExisted.Name);
            return jobExisted;
        }

        public static void DeleteJob(String jobName)
        {
            jobServiceClient.Projects.Jobs.Delete(jobName).Execute();
            Console.WriteLine("Job deleted: " + jobName);
        }

        public static void SetupCompany()
        {
            myCompany = new Company()
            {
                DisplayName = "Google",
                HeadquartersAddress = "1600 Ampitheatre Parkway, Mountain View CA 94043",
                ExternalId = generateRandom()
            };
            myCompany = CreateCompany(myCompany);
        }

        public static Company CreateCompany(Company companyToBeCreated)
        {
            // Start method
            CreateCompanyRequest createCompanyRequest = new CreateCompanyRequest();
            createCompanyRequest.Company = companyToBeCreated;
            Company companyCreated = jobServiceClient.Projects.Companies.Create(createCompanyRequest, parent).Execute();
            Console.WriteLine("Created company: " + ToJsonString(companyCreated));
            return companyCreated;
        }

        public static Company GetCompany(string companyName)
        {
            try
            {
                Company companyExisted = jobServiceClient.Projects.Companies.Get(companyName).Execute();
                Console.WriteLine("Company existed: " + companyExisted.Name);
                return companyExisted;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public static void DeleteCompany(string companyName)
        {
            jobServiceClient.Projects.Companies.Delete(companyName).Execute();
            Console.WriteLine("Deleted company: " + companyName);
        }

        public static String generateRandom()
        {
            Random random = new Random();
            int randomId = random.Next(1000000, 10000000);
            return randomId.ToString();
        }

        private static string ToJsonString(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
