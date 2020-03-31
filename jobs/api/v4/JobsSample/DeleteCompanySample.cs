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

<<<<<<< HEAD
using Google.Cloud.Talent.V4Beta1;
using System;
=======
using System;
using Google.Cloud.Talent.V4Beta1;
>>>>>>> 8abacdfaba62df07bc3749642014fd30b63c95a6

namespace GoogleCloudSamples
{
    internal class DeleteCompanySample
    {
        // [START job_search_delete_company_beta]
        public static object DeleteCompany(string projectId, string tenantId, string companyId)
        {
            CompanyServiceClient companyServiceClient = CompanyServiceClient.Create();
            string companyName = CompanyName.Format(projectId, tenantId, companyId);
            DeleteCompanyRequest request = new DeleteCompanyRequest
            {
                Name = companyName
            };

            companyServiceClient.DeleteCompany(request);

            Console.WriteLine("Deleted Company.");
            return 0;
        }
        // [END job_search_delete_company_beta]
    }
}
