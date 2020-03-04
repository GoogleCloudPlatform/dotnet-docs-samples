using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;
using Xunit;

public class QuickStartNewTest : IDisposable
{
    private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private ServiceAccount serviceAccount;
    private IamService iamService;

    public QuickStartNewTest()
    {
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
            AccountId = "iam-test-account",
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
    public void TestQuickStartNew()
    {
        var role = "roles/logging.logWriter";
        var rolePermissions = new List<string>() { "logging.logEntries.create" };
        var member = "serviceAccount:" + serviceAccount.Email;

        // Initialize service
        var crmService = QuickStartNew.InitializeService();

        // Add member to role
        QuickStartNew.AddBinding(crmService, _projectId, member, role);

        // Test permissions in role
        var grantedPermissions = new List<string>(QuickStartNew.TestPermissions(crmService, _projectId, member, rolePermissions));

        // Verify that all permissions in role were granted to member
        foreach (var p in rolePermissions)
        {
            Assert.Contains(p, grantedPermissions);
        }

        // Remove member
        QuickStartNew.RemoveMember(crmService, _projectId, member, role);
    }
}
