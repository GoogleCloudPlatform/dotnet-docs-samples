using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class ServiceAccountTests
    {
        [Fact]
        public void TestServiceAccounts()
        {
            string projectId = Environment.GetEnvironmentVariable(
                "GOOGLE_PROJECT_ID");
            var rand = new Random().Next(0, 1000);
            string name = "dotnet-test-" + rand;
            string email = $"{name}@{projectId}.iam.gserviceaccount.com";

            ServiceAccounts.Init();
            ServiceAccounts.CreateServiceAccount(projectId, name, "C# Test Account");
            ServiceAccounts.ListServiceAccounts(projectId);
            ServiceAccounts.RenameServiceAccount(email, "Updated C# Test Account");

            ServiceAccountKeys.Init();
            var key = ServiceAccountKeys.CreateKey(email);
            ServiceAccountKeys.ListKeys(email);
            ServiceAccountKeys.DeleteKey(key.Name);

            ServiceAccounts.DeleteServiceAccount(email);
        }
    }
}