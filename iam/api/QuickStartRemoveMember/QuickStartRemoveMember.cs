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

// [START iam_quickstart_remove]

using System.Linq;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.CloudResourceManager.v1.Data;

public class QuickStartRemoveMember
{
    public static void Main(string[] args)
    {
        //TODO: Replace with your project ID
        var projectId = "your-project";
        //TODO: Replace with the ID of the member who was granted the
        // Logs Viewer role
        var member1 = "your-member";
        var role = "roles/logging.viewer";

        // Initialize service
        var crmService = InitializeService();

        // Remove your member the "Logs Viewer" role for your project
        RemoveMember(crmService, projectId, member1, role);
    }

    public static CloudResourceManagerService InitializeService(){
        
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
    public static void RemoveMember(CloudResourceManagerService crmService,
                                 string projectId, string member, string role)
    {
        // Get the project's policy by calling the Cloud Resource Manager
        // Projects API
        var policy = crmService.Projects.GetIamPolicy(new GetIamPolicyRequest(),
            projectId).Execute();

        // Remove the member from the role
        try
        {
            var binding = policy.Bindings.First(x => x.Role == role);
            if (binding.Members.Count != 0 && binding.Members.Contains(member))
            {
                binding.Members.Remove(member);
            }
            if (binding.Members.Count == 0)
            {
                policy.Bindings.Remove(binding);
            }
        }
        catch (System.InvalidOperationException e)
        {
            System.Diagnostics.Debug.WriteLine("Role does not exist in policy: \n" + e.ToString());
        }

        // Set the project's policy by calling the Cloud Resource Manager
        // Projects API
        crmService.Projects.SetIamPolicy(new SetIamPolicyRequest
        {
            Policy = policy
        }, projectId).Execute();
    }
}
// [END iam_quickstart_remove]