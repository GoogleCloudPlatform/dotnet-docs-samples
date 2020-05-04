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

// [START iam_quickstart_v2]

using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.CloudResourceManager.v1.Data;
using Google.Apis.Iam.v1;
using System;
using System.Collections.Generic;
using System.Linq;


public class QuickStart
{
    public static void Main(string[] args)
    {
        // TODO: Replace with your project ID
        var projectId = "your-project";
        // TODO: Replace with the ID of the service account used in this quickstart in
        // the form "serviceAccount:[service-account-id]@[project-id].iam.gserviceaccount.com"
        var member = "your-service-account";
        // Role to be granted
        var role = "roles/logging.logWriter";
        // All permissions contained in the role to be granted
        var rolePermissions = new List<string> { "logging.logEntries.create" };

        // Initialize service
        CloudResourceManagerService crmService = InitializeService();

        // Grant your member the "Log writer" role for your project
        AddBinding(crmService, projectId, member, role);
        // Test if the member has the permissions granted by the role
        IList<string> grantedPermissions = TestPermissions(crmService,
                                                           projectId,
                                                           rolePermissions);
        // Print the role permissions held by the member
        foreach (var p in grantedPermissions)
        {
            Console.WriteLine(p);
        }
        // Remove member from the "Log writer" role
        RemoveMember(crmService, projectId, member, role);
    }

    public static CloudResourceManagerService InitializeService()
    {
        // Get credentials
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);

        // Create the Cloud Resource Manager service object
        CloudResourceManagerService crmService = new CloudResourceManagerService(
            new CloudResourceManagerService.Initializer
            {
                HttpClientInitializer = credential
            });

        return crmService;
    }

    public static void AddBinding(
        CloudResourceManagerService crmService,
        string projectId,
        string member,
        string role)
    {
        // Get the project's policy by calling the
        // Cloud Resource Manager Projects API
        var policy = crmService.Projects.GetIamPolicy(
            new GetIamPolicyRequest(),
            projectId).Execute();

        // Find binding in policy
        var binding = policy.Bindings.FirstOrDefault(x => x.Role == role);

        // If binding already exists, add member to binding
        if (binding != null)
        {
            binding.Members.Add(member);
        }
        // If binding does not exist, add binding to policy
        else
        {
            binding = new Binding
            {
                Role = role,
                Members = new List<string> { member }
            };
            policy.Bindings.Add(binding);
        }

        // Set the project's policy by calling the
        // Cloud Resource Manager Projects API
        crmService.Projects.SetIamPolicy(
           new SetIamPolicyRequest
           {
               Policy = policy
           }, projectId).Execute();
    }


    public static IList<string> TestPermissions(
        CloudResourceManagerService crmService,
        string projectId,
        List<string> permissions)
    {
        //Test the member's permissions by calling the
        //Cloud Resource Manager Projects API
        TestIamPermissionsRequest requestBody = new TestIamPermissionsRequest()
        {
            permissions = permissions
        };
        var returnedPermissions = crmService.Projects.TestIamPermissions(
            requestBody, projectId).Execute().Permissions;

        return returnedPermissions;
    }

    public static void RemoveMember(
        CloudResourceManagerService crmService,
        string projectId,
        string member,
        string role)
    {
        // Get the project's policy by calling the
        // Cloud Resource Manager Projects API
        var policy = crmService.Projects.GetIamPolicy(
            new GetIamPolicyRequest(),
            projectId).Execute();

        // Remove the member from the role
        var binding = policy.Bindings.FirstOrDefault(x => x.Role == role);
        if (binding == null)
        {
            Console.WriteLine("Role does not exist in policy.");
        }
        else
        {
            if (binding.Members.Contains(member))
            {
                binding.Members.Remove(member);
            }
            else
            {
                Console.WriteLine("The member has not been granted this role.");
            }

            if (binding.Members.Count == 0)
            {
                policy.Bindings.Remove(binding);
            }
        }

        // Set the project's policy by calling the
        // Cloud Resource Manager Projects API
        crmService.Projects.SetIamPolicy(
            new SetIamPolicyRequest
            {
                Policy = policy
            }, projectId).Execute();
    }
}
// [END iam_quickstart_v2]