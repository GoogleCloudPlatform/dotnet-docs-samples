// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class CreateCompanySample
    {
        // [START job_search_create_company_beta]
        public static object CreateCompany(string projectId, string tenantId, string displayName, string externalId)
        {
            CompanyServiceClient companyServiceClient = CompanyServiceClient.Create();
            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);

            Company company = new Company
            {
                DisplayName = displayName,
                ExternalId = externalId
            };

            CreateCompanyRequest request = new CreateCompanyRequest
            {
                ParentAsTenantName = tenantName,
                Company = company
            };

            Company response = companyServiceClient.CreateCompany(request);

            Console.WriteLine("Created Company");
            Console.WriteLine($"Name: {response.Name}");
            Console.WriteLine($"Display Name: {response.DisplayName}");
            Console.WriteLine($"External ID: {response.ExternalId}");
            return 0;
        }
        // [END job_search_create_company_beta]
    }
}
