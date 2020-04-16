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
    internal class ListCompaniesSample
    {
        // [START job_search_list_companies_beta]
        public static object ListCompanies(string projectId, string tenantId)
        {
            CompanyServiceClient companyServiceClient = CompanyServiceClient.Create();
            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);

            ListCompaniesRequest request = new ListCompaniesRequest
            {
                ParentAsTenantName = tenantName
            };
            var companies = companyServiceClient.ListCompanies(request);
            foreach (var company in companies)
            {
                Console.WriteLine($"Company Name: {company.Name}");
                Console.WriteLine($"Display Name: {company.DisplayName}");
                Console.WriteLine($"External ID: {company.ExternalId}");
            }
            return 0;
        }
        // [END job_search_list_companies_beta]
    }
}
