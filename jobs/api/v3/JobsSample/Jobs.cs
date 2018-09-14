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
    public class Jobs
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
            ApplicationInfo info = new ApplicationInfo()
            {
                Instruction = "Application instructions here"
            };
            Job newJob = new Job()
            {
                Title = "Software Engineer",
                RequisitionId = generateRandom(),
                Description = "My Job Description Here",
                CompanyName = myCompany.Name,
                ApplicationInfo = info
            };

            newJob = CreateJob(newJob);

            newJob.Title = "Software Engineer II";
            newJob = UpdateJob(newJob.Name, newJob);

            newJob.Title = "Software Engineer III";
            newJob = UpdateJobWithFieldMask(newJob.Name, "Title", newJob);


            DeleteJob(newJob.Name);

            DeleteCompany(myCompany.Name);
        }

        // [START create_job]

        public static Job CreateJob(Job jobToBeCreated)
        {
            try
            {
                CreateJobRequest createJobRequest = new CreateJobRequest();
                createJobRequest.Job = jobToBeCreated;

                Job jobCreated = jobServiceClient.Projects.Jobs.Create(createJobRequest, parent).Execute();
                Console.WriteLine("Job created: " + jobCreated.Name);
                return jobCreated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while creating job");
                throw e;
            }
        }
        // [END create_job]

        // [START custom_attribute_job]
        public static Job GenerateJobWithACustomAttribute(string companyName)
        {
            // Requisition id should be a unique Id in your system
            string requisitionId = "jobWithCustomAttribute:" + new Random().Next().ToString();

            string jobTitle = "Software Engineer";
            string applicationURL = "http://careers.google.com";
            string description = "Design, develop, test, deploy, maintain and inmprove software";


            //Construct custom attributes map
            CustomAttribute customAttribute = new CustomAttribute();
            customAttribute.StringValues = new List<String>()
            {
                "value1"
            };

            Dictionary<String, CustomAttribute> customAttributes = new Dictionary<string, CustomAttribute>
            {
                { "customField1", customAttribute }
            };

            ApplicationInfo applicationInfo = new ApplicationInfo()
            {
                Uris = new List<String>
                {
                    applicationURL
                }
            };

            Job customJob = new Job()
            {
                Title = jobTitle,
                RequisitionId = requisitionId,
                Description = description,
                CompanyName = companyName,
                ApplicationInfo = applicationInfo,
                CustomAttributes = customAttributes
            };

            Console.WriteLine("Job generated:" + customJob.Title);
            return customJob;
        }
        // [END custom_attribute_job]

        // [START get_job]

        public static Job GetJob(String jobName)
        {
            try
            {
                Job jobExisted = jobServiceClient.Projects.Jobs.Get(jobName).Execute();
                Console.WriteLine("Job exists: " + jobExisted.Name);
                return jobExisted;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while getting job");
                throw e;
            }
        }
        // [END get_job]

        // [START update_job]

        public static Job UpdateJob(string jobName, Job toBeUpdated)
        {
            try
            {
                UpdateJobRequest updateJobRequest = new UpdateJobRequest()
                {
                    Job = toBeUpdated
                };
                Job jobUpdated = jobServiceClient.Projects.Jobs.Patch(updateJobRequest, jobName).Execute();
                Console.WriteLine("Job updated: " + ToJsonString(jobUpdated));
                return jobUpdated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while updating job");
                throw e;
            }
        }
        // [END update_job]

        // [START update_job_with_field_mask]

        public static Job UpdateJobWithFieldMask(string jobName, string fieldMask, Job jobToBeUpdated)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine("Got exception while updating job with field mask");
                throw e;
            }
        }
        // [END update_job_with_field_mask]

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
