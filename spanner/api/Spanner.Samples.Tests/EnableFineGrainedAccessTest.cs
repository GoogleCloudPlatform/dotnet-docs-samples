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
using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class EnableFineGrainedAccessTest : IDisposable
{
    private readonly string ServiceAccountId = $"FgacTestAccount{Guid.NewGuid().ToString().Substring(0, 10)}";
    private readonly string ServiceAccountDisplayName = $"FgacTestAccount{Guid.NewGuid().ToString().Substring(0, 10)}";
    private readonly SpannerFixture _spannerFixture;
    private readonly ServiceAccount _serviceAccount;
    private readonly IamService _service; 

    public EnableFineGrainedAccessTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        var credential = GoogleCredential.GetApplicationDefault().CreateScoped(IamService.Scope.CloudPlatform);
        _service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });
        _serviceAccount = CreateServiceAccount(_service);
    }

    private ServiceAccount CreateServiceAccount(IamService service)


    {
        var request = new CreateServiceAccountRequest
        {
            AccountId = ServiceAccountId,
            ServiceAccount = new ServiceAccount
            {
                DisplayName = ServiceAccountDisplayName
            }
        };
        return service.Projects.ServiceAccounts.Create(
            request, $"projects/{_spannerFixture.ProjectId}").Execute();

    }

    private static void DeleteServiceAccount(IamService service, string serviceAccountName)

    {
        try
        {
            service.Projects.ServiceAccounts.Delete(serviceAccountName).Execute();
        }
        catch
        {
            // No op. We delete on a best effort basis.
        }
    }


    [Fact]
    public async Task TestEnableFineGrainedAccessAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(databaseId =>
        {
            string databaseRole = "testrole";
            var enableFineGrainedAccessSample = new EnableFineGrainedAccessSample();
            var updatedPolicy = enableFineGrainedAccessSample.EnableFineGrainedAccess(_spannerFixture.ProjectId, _spannerFixture.InstanceId,
                databaseId, databaseRole, $"serviceAccount:{_serviceAccount.Email}");
            Assert.Contains(updatedPolicy.Bindings, b => b.Role == "roles/spanner.fineGrainedAccessUser");
            return Task.CompletedTask;
        });
    }
    public void Dispose() => 
        DeleteServiceAccount(_service, _serviceAccount.Name);
}
