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

// [START iam_quickstart]

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
        // TODO: Replace with the ID of your principal.
        // For examples, see https://cloud.google.com/iam/docs/principal-identifiers
        var member = "your-principal";
        // Role to be granted
        var role = "roles/logging.logWriter";

        // Initialize service
        CloudResourceManagerService crmService = InitializeService();

        // Grant your principal the "Log Writer" role for your project
        AddBinding(crmService, projectId, member, role);

        // Get the project's policy and print all principals with the the "Log Writer" role
        var policy = GetPolicy(crmService, projectId);
        var binding = policy.Bindings.FirstOrDefault(x => x.Role == role);
        Console.WriteLine("Role: " + binding.Role);
        Console.Write("Members: ");
        foreach (var m in binding.Members)
        {
            Console.Write("[" + m + "] ");
        }
        Console.WriteLine();

        // Remove principal from the "Log Writer" role
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

    public static Policy GetPolicy(CloudResourceManagerService crmService, String projectId)
    {
        // Get the project's policy by calling the
        // Cloud Resource Manager Projects API
        var policy = crmService.Projects.GetIamPolicy(
            new GetIamPolicyRequest(),
            projectId).Execute();
        return policy;
    }

    public static void SetPolicy(CloudResourceManagerService crmService, String projectId, Policy policy)
    {
        // Set the project's policy by calling the
        // Cloud Resource Manager Projects API
        crmService.Projects.SetIamPolicy(
           new SetIamPolicyRequest
           {
               Policy = policy
           }, projectId).Execute();
    }

    public static void AddBinding(
        CloudResourceManagerService crmService,
        string projectId,
        string member,
        string role)
    {
        // Get the project's policy
        var policy = GetPolicy(crmService, projectId);

        // Find binding in policy
        var binding = policy.Bindings.FirstOrDefault(x => x.Role == role);

        // If binding already exists, add principal to binding
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

        // Set the updated policy
        SetPolicy(crmService, projectId, policy);
    }

    public static void RemoveMember(
        CloudResourceManagerService crmService,
        string projectId,
        string member,
        string role)
    {
        // Get the project's policy 
        var policy = GetPolicy(crmService, projectId);

        // Remove the principal from the role
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

        // Set the updated policy
        SetPolicy(crmService, projectId, policy);
    }
}
// [END iam_quickstart]
