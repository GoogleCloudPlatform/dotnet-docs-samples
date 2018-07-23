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

// [START iam_quickstart]

using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

public class QuickStart
{
    public static void Main(string[] args)
    {
        // Get credentials
        GoogleCredential credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);

        // Create the Cloud IAM service object
        IamService service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        // Call the Cloud IAM Roles API
        ListRolesResponse response = service.Roles.List().Execute();
        IList<Role> roles = response.Roles;

        // Process the response
        foreach (Role role in roles)
        {
            Console.WriteLine("Title: " + role.Title);
            Console.WriteLine("Name: " + role.Name);
            Console.WriteLine("Description: " + role.Description);
            Console.WriteLine();
        }
    }
}
// [END iam_quickstart]
