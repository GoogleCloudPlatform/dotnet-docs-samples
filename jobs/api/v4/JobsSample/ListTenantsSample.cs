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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class ListTenantsSample
    {
        // [START job_search_list_tenants_beta]
        public static object ListTenants(string projectId)
        {
            TenantServiceClient tenantServiceClient = TenantServiceClient.Create();
            ProjectName parent = new ProjectName(projectId);
            ListTenantsRequest request = new ListTenantsRequest
            {
                ParentAsProjectName = parent
            };
            var tenants = tenantServiceClient.ListTenants(request);
            foreach (var tenant in tenants)
            {
                Console.WriteLine($"Tenant Name: {tenant.Name}");
                Console.WriteLine($"External ID: {tenant.ExternalId}");
            }
            return 0;
        }
        // [END job_search_list_tenants_beta]
    }
}
