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

// [START iam_delete_service_account]

using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;

public partial class ServiceAccounts
{
    public static void DeleteServiceAccount(string email)
    {
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        string resource = "projects/-/serviceAccounts/" + email;
        service.Projects.ServiceAccounts.Delete(resource).Execute();
        Console.WriteLine("Deleted service account: " + email);
    }
}
// [END iam_delete_service_account]