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
    internal class DeleteTenantSample
    {
        // [START job_search_delete_tenant_beta]
        public static object DeleteTenant(string projectId, string tenantId)
        {
            TenantServiceClient tenantServiceClient = TenantServiceClient.Create();
            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);
            DeleteTenantRequest request = new DeleteTenantRequest
            {
                TenantName = tenantName
            };
            tenantServiceClient.DeleteTenant(request);
            Console.WriteLine($"Deleted Tenant.");
            return 0;
        }
        // [END job_search_delete_tenant_beta]
    }
}
