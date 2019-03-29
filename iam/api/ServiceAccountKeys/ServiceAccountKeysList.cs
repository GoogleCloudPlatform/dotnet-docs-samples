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

// [START iam_list_keys]

using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

public partial class ServiceAccountKeys
{
    public static IList<ServiceAccountKey> ListKeys(string serviceAccountEmail)
    {
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        var response = service.Projects.ServiceAccounts.Keys
            .List($"projects/-/serviceAccounts/{serviceAccountEmail}")
            .Execute();
        foreach (ServiceAccountKey key in response.Keys)
        {
            Console.WriteLine("Key: " + key.Name);
        }
        return response.Keys;
    }
}
// [END iam_list_keys]