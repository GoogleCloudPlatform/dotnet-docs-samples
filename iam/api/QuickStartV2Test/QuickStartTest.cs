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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;
using System;
using System.Collections.Generic;
using Xunit;

public class QuickStartTest : IDisposable
{
    private readonly string projectId;
    private ServiceAccount serviceAccount;
    private IamService iamService;

    public QuickStartTest()
    {
        // Check for _projectId and throw exception if empty
        projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (projectId == null)
        {
            throw new System.ArgumentNullException("GOOGLE_PROJECT_ID", "Environment variable not set");
        }

        // Create service account for test
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);
        iamService = new IamService(
            new IamService.Initializer
            {
                HttpClientInitializer = credential
            });

        var request = new CreateServiceAccountRequest
        {
            AccountId = "iam-test-account" + DateTime.UtcNow.ToBinary(),
            ServiceAccount = new ServiceAccount
            {
                DisplayName = "iamTestAccount"
            }
        };
        serviceAccount = iamService.Projects.ServiceAccounts.Create(
            request, "projects/" + _projectId).Execute();
    }

    public void Dispose()
    {
        // Delete service account
        string resource = "projects/-/serviceAccounts/" + serviceAccount.Email;
        iamService.Projects.ServiceAccounts.Delete(resource).Execute();

    }

    [Fact]
    public void TestQuickStart()
    {
        var role = "roles/logging.logWriter";
        var rolePermissions = new List<string> { "logging.logEntries.create" };
        var member = "serviceAccount:" + serviceAccount.Email;

        // Initialize service
        var crmService = QuickStart.InitializeService();

        // Add member to role
        QuickStart.AddBinding(crmService, _projectId, member, role);

        // Test permissions in role
        var grantedPermissions = QuickStart.TestPermissions(crmService, _projectId, member, rolePermissions);

        // Verify that all permissions in role were granted to member
        foreach (var p in rolePermissions)
        {
            Assert.Contains(p, grantedPermissions);
        }

        // Remove member
        QuickStart.RemoveMember(crmService, _projectId, member, role);
    }
}
