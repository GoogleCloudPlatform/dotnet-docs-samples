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
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudTalentSolution.v3;
using Google.Apis.CloudTalentSolution.v3.Data;
using Google.Apis.Services;

namespace GoogleCloudSamples
{
    public class Companies
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

            myCompany.DisplayName = "Updated Company Name";
            myCompany = UpdateCompany(myCompany.Name, myCompany);

            myCompany.DisplayName = "Updated Company UpdateMask";
            myCompany = UpdateCompanyWithFieldMask(myCompany.Name, "DisplayName", myCompany);

            DeleteCompany(myCompany.Name);
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

        // [START create_company]

        public static Company CreateCompany(Company companyToBeCreated)
        {
            try
            {
                CreateCompanyRequest createCompanyRequest = new CreateCompanyRequest();
                createCompanyRequest.Company = companyToBeCreated;
                Company companyCreated = jobServiceClient.Projects.Companies.Create(createCompanyRequest, parent).Execute();
                Console.WriteLine("Created company: " + ToJsonString(companyCreated));
                return companyCreated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while creating company");
                throw e;
            }
        }
        // [END create_company]

        // [START get_company]

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
                Console.WriteLine("Got exception while getting company");
                throw e;
            }
        }
        // [END get_company]

        // [START update_company]

        public static Company UpdateCompany(String companyName, Company companyToBeUpdated)
        {
            try
            {
                UpdateCompanyRequest updateCompanyRequest = new UpdateCompanyRequest()
                {
                    Company = companyToBeUpdated
                };
                Company companyUpdated = jobServiceClient.Projects.Companies.Patch(updateCompanyRequest, companyName).Execute();
                Console.WriteLine("Company updated: " + ToJsonString(companyUpdated));
                return companyUpdated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while updating company");
                throw e;
            }
        }
        // [END update_company]

        // [START update_company_with_field_mask]

        public static Company UpdateCompanyWithFieldMask(string companyName, string fieldMask, Company companyToBeUpdated)
        {
            try
            {
                UpdateCompanyRequest updateCompanyRequest = new UpdateCompanyRequest()
                {
                    UpdateMask = fieldMask,
                    Company = companyToBeUpdated
                };
                Company companyUpdated = jobServiceClient.Projects.Companies.Patch(updateCompanyRequest, companyName).Execute();
                Console.WriteLine("Company updated with updateMask: " + ToJsonString(companyUpdated));
                return companyUpdated;
            }
            catch (Exception e)
            {
                Console.WriteLine("Got exception while updating company with update mask");
                throw e;
            }
        }
        // [END update_company_with_field_mask]

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
