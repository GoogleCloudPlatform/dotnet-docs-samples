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

// [START quickstart]

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
// Imports the Google Cloud Job Discovery client library
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;
using System.Linq;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    Google.Apis.JobService.v2.JobServiceService.Scope.Jobs
                });
            }
            // Instantiate the Cloud Key Management Service API.
            JobServiceService jobServiceClient = new JobServiceService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            // List companies.
            ListCompaniesResponse result = jobServiceClient.Companies.List().Execute();
            Console.WriteLine("Request Id: " + result.Metadata.RequestId);
            if (result.Companies != null)
            {
                Console.WriteLine("Companies: ");
                result.Companies.ToList().ForEach(company => Console.WriteLine(company.Name));
            }
            else { Console.WriteLine("No companies found."); }
        }
    }
}
// [END quickstart]