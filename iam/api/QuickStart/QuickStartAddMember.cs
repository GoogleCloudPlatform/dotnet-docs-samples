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

// [START iam_quickstart_add]

using System.Linq;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.CloudResourceManager.v1.Data;

public class QuickStartAddMember
{
    public static void Main(string[] args)
    {
        //TODO: Replace with your project ID
        var projectId = "your-project";
        //TODO: Replace with a member ID in the form `user:member@example.com`
        var member = "your-member";
        var role = "roles/logging.viewer";

        // Initialize service
        var crmService = InitializeService();

        // Grant your member the "Logs Viewer" role for your project
        AddBinding(crmService, projectId, member, role);
    }

    public static CloudResourceManagerService InitializeService()
    {
        // Get credentials
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);

        // Create the Cloud Resource Manager service object
        var crmService = new CloudResourceManagerService(
            new CloudResourceManagerService.Initializer
            {
                HttpClientInitializer = credential
            });

        return crmService;
    }

    public static void AddBinding(CloudResourceManagerService crmService,
                                 string projectId, string member, string role)
    {
        // Get the project's policy by calling the Cloud Resource Manager
        // Projects API
        var policy = crmService.Projects.GetIamPolicy(new GetIamPolicyRequest(),
            projectId).Execute();

        // Find binding in policy
        var binding = policy.Bindings.First(x => x.Role == role);

        // If binding already exists, add member to binding
        if (binding != null)
        {
          binding.Members.Add(member);
        }
        // If binding does not exist, add binding to policy
        else{
          binding = new Binding
          {
              Role = role,
              Members = new List<string> { member }
          };
          policy.Bindings.Add(binding);
        }

        // Set the project's policy by calling the Cloud Resource Manager
        // Projects API
        crmService.Projects.SetIamPolicy(new SetIamPolicyRequest
        {
            Policy = policy
        }, projectId).Execute();
    }
}
// [END iam_quickstart_add]