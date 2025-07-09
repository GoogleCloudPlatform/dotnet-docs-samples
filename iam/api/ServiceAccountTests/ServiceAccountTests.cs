// Copyright 2018 Google Inc.
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
using System;
using System.Net;
using Xunit;

namespace GoogleCloudSamples
{
    public class ServiceAccountTests
    {
        [Fact]
        public void TestServiceAccounts()
        {
            // Reading service accounts is eventually consistent so we might need to
            // retry the first operation that fetches the account.
            RetryRobot saCreated = new RetryRobot
            {
                FirstRetryDelayMs = 10 * 1000,
                MaxTryCount = 10,
                ShouldRetry = ex => ex is GoogleApiException gEx && gEx.HttpStatusCode == HttpStatusCode.NotFound
            };
            
            string projectId = Environment.GetEnvironmentVariable(
                "GOOGLE_PROJECT_ID");
            string name = "t" + Guid.NewGuid().ToString("N").Substring(0, 29);
            string email = $"{name}@{projectId}.iam.gserviceaccount.com";

            ServiceAccounts.CreateServiceAccount(projectId, name, "C# Test Account");
            ServiceAccounts.ListServiceAccounts(projectId);
            saCreated.Eventually(() => ServiceAccounts.RenameServiceAccount(email, "Updated C# Test Account"));

            var key = ServiceAccountKeys.CreateKey(email);
            ServiceAccountKeys.ListKeys(email);
            ServiceAccountKeys.DeleteKey(key.Name);

            ServiceAccounts.DisableServiceAccount(email);
            ServiceAccounts.EnableServiceAccount(email);

            ServiceAccounts.DeleteServiceAccount(email);
        }
    }
}
