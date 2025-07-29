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

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace GoogleCloudSamples
{
    public class QuickStartTest : IDisposable
    {
        private readonly string _projectId;
        private readonly ServiceAccount _serviceAccount;
        private readonly IamService _iamService;

        public QuickStartTest()
        {
            // Check for _projectId and throw exception if empty
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            if (_projectId == null)
            {
                throw new ArgumentNullException("GOOGLE_PROJECT_ID", "Environment variable not set");
            }

            // Create service account for test
            var credential = GoogleCredential.GetApplicationDefault()
                .CreateScoped(IamService.Scope.CloudPlatform);
            _iamService = new IamService(
                new IamService.Initializer
                {
                    HttpClientInitializer = credential
                });

            var request = new CreateServiceAccountRequest
            {
                AccountId = "t" + Guid.NewGuid().ToString("N").Substring(0, 29),
                ServiceAccount = new ServiceAccount
                {
                    DisplayName = "iamTestAccount"
                }
            };
            _serviceAccount = _iamService.Projects.ServiceAccounts.Create(
                request, "projects/" + _projectId).Execute();
        }

        public void Dispose()
        {
            // Delete service account
            string resource = "projects/-/serviceAccounts/" + _serviceAccount.Email;
            _iamService.Projects.ServiceAccounts.Delete(resource).Execute();

        }

        [Fact]
        public void TestQuickStart()
        {
            var role = "roles/logging.logWriter";
            var member = "serviceAccount:" + _serviceAccount.Email;

            // Reading service accounts is eventually consistent so we might need to
            // retry the first operation that fetches the account.
            RetryRobot saCreated = new RetryRobot
            {
                FirstRetryDelayMs = 10 * 1000,
                MaxTryCount = 10,
                ShouldRetry = ex =>
                    ex is GoogleApiException gEx
                    && gEx.HttpStatusCode == HttpStatusCode.BadRequest
                    && gEx.Message.Contains("does not exist")
            };

            // Initialize service
            var crmService = QuickStart.InitializeService();

            // Add member to role
            saCreated.Eventually(() => QuickStart.AddBinding(crmService, _projectId, member, role));

            // Get the project's policy and confirm that the member is in the policy
            var policy = QuickStart.GetPolicy(crmService, _projectId);
            var binding = policy.Bindings.FirstOrDefault(x => x.Role == role);
            Assert.Contains(member, binding.Members);

            // Remove member
            QuickStart.RemoveMember(crmService, _projectId, member, role);
            // Confirm that the member has been removed
            policy = QuickStart.GetPolicy(crmService, _projectId);
            binding = policy.Bindings.FirstOrDefault(x => x.Role == role);
            if (binding != null)
            {
                Assert.DoesNotContain(member, binding.Members);
            }
        }
    }
}
