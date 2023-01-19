// Copyright 2023 Google Inc.
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
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class EnableFineGrainedAccessTest
{
    private const string ServiceAccountId = "SpannerFGACTestAccount";
    private const string ServiceAccountDisplayName = "SpannerFGACTest";
    private readonly SpannerFixture _spannerFixture;

    public EnableFineGrainedAccessTest(SpannerFixture spannerFixture) =>
        _spannerFixture = spannerFixture;

    private ServiceAccount CreateServiceAccount()
    {
        var credential = GoogleCredential.GetApplicationDefault().CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = new CreateServiceAccountRequest
        {
            AccountId = ServiceAccountId,
            ServiceAccount = new ServiceAccount
            {
                DisplayName = ServiceAccountDisplayName
            }
        };
        var serviceAccount = service.Projects.ServiceAccounts.Create(
            request, $"projects/{_spannerFixture.ProjectId}").Execute();
        return serviceAccount;
    }

    private void DeleteServiceAccount(string ServiceAccountName)
    {
        var credential = GoogleCredential.GetApplicationDefault().CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });
        service.Projects.ServiceAccounts.Delete(ServiceAccountName).Execute();
    }

    [Fact]
    public async Task TestEnableFineGrainedAccessAsync()
    {
        var serviceAccount = CreateServiceAccount();
        try
        {
            await _spannerFixture.RunWithTemporaryDatabaseAsync(databaseId =>
            {
                string databaseRole = "testrole";
                var enableFineGrainedAccessSample = new EnableFineGrainedAccessSample();
                var updatedPolicy = enableFineGrainedAccessSample.EnableFineGrainedAccess(_spannerFixture.ProjectId, _spannerFixture.InstanceId,
                    databaseId, databaseRole, $"serviceAccount:{serviceAccount.Email}");
                Assert.Contains(updatedPolicy.Bindings, b => b.Role == "roles/spanner.fineGrainedAccessUser");
                return Task.CompletedTask;
            });
        }
        finally
        {
            DeleteServiceAccount(serviceAccount.Name);
        }
    }
}
