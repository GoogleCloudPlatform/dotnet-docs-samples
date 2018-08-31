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
using Google.Apis.Requests;
using Google.Apis.Services;

namespace GoogleCloudSamples
{
    public class BatchOperations
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

            List<Job> createdJobs = BatchCreateJob(myCompany.Name);
            foreach (Job job in createdJobs)
            {
                Console.WriteLine("Created job: " + job.Title);
            }

            foreach (Job job in createdJobs)
            {
                DeleteJob(job.Name);
            }

            DeleteCompany(myCompany.Name);
        }

        // [START batch_job_create]
        public static List<Job> BatchCreateJob(string companyName)
        {
            List<Job> createdJobs = new List<Job>();

            ApplicationInfo applicationInfo = new ApplicationInfo()
            {
                Uris = new List<string>
                {
                    "http://careers.google.com"
                }
            };

            Job softwareEngineerJob = new Job()
            {
                CompanyName = companyName,
                RequisitionId = generateRandom(),
                Title = "Software Engineer",
                ApplicationInfo = applicationInfo,
                Description = "Design, develop, test, depoloy, maintain and improve software."
            };

            Job hardwareEngineerJob = new Job()
            {
                CompanyName = companyName,
                RequisitionId = generateRandom(),
                Title = "Hardware Engineer",
                ApplicationInfo = applicationInfo,
                Description = "Design and prototype PCDs or modify existing board designs."
            };

            CreateJobRequest firstJobRequest = new CreateJobRequest()
            {
                Job = softwareEngineerJob
            };

            CreateJobRequest secondJobRequest = new CreateJobRequest()
            {
                Job = hardwareEngineerJob
            };

            BatchRequest batchCreate = new BatchRequest(jobServiceClient);

            batchCreate.Queue<Job>(jobServiceClient.Projects.Jobs.Create(firstJobRequest, parent),
                (content, error, i, message) =>
                {
                    if (error == null)
                    {
                        createdJobs.Add(content);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + error);
                    }
                });
            batchCreate.Queue<Job>(jobServiceClient.Projects.Jobs.Create(secondJobRequest, parent),
                (content, error, i, message) =>
                {
                    if (error == null)
                    {
                        createdJobs.Add(content);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + error);
                    }
                });

            batchCreate.ExecuteAsync().Wait();
            return createdJobs;
        }
        // [END batch_job_create]

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
