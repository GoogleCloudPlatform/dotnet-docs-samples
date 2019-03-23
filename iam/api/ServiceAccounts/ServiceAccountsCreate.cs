// Copyright 2018 Google Inc.
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

// [START iam_create_service_account]

using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

public partial class ServiceAccounts
{
    public static ServiceAccount CreateServiceAccount(string projectId,
        string name, string displayName)
    {
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = new CreateServiceAccountRequest
        {
            AccountId = name,
            ServiceAccount = new ServiceAccount
            {
                DisplayName = displayName
            }
        };
        var serviceAccount = service.Projects.ServiceAccounts.Create(
            request, "projects/" + projectId).Execute();
        Console.WriteLine("Created service account: " + serviceAccount.Email);
        return serviceAccount;
    }
}
// [END iam_create_service_account]