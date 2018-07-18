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

using System;
using System.Collections.Generic;
using CommandLine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

namespace GoogleCloudSamples
{
    public static class ServiceAccounts
    {
        private static IamService s_iam;

        // [START iam_create_service_account]
        public static ServiceAccount CreateServiceAccount(string projectId,
            string name, string displayName)
        {
            var request = new CreateServiceAccountRequest
            {
                AccountId = name,
                ServiceAccount = new ServiceAccount
                {
                    DisplayName = displayName
                }
            };

            ServiceAccount serviceAccount = s_iam.Projects.ServiceAccounts
                .Create(request, "projects/" + projectId).Execute();

            Console.WriteLine("Created service account: " + serviceAccount.Email);
            return serviceAccount;
        }
        // [END iam_create_service_account]

        // [START iam_list_service_accounts]
        public static IList<ServiceAccount> ListServiceAccounts(string projectId)
        {
            ListServiceAccountsResponse response = s_iam.Projects.ServiceAccounts
                .List("projects/" + projectId).Execute();
            IList<ServiceAccount> serviceAccounts = response.Accounts;

            foreach (ServiceAccount account in serviceAccounts)
            {
                Console.WriteLine("Name: " + account.Name);
                Console.WriteLine("Display Name: " + account.DisplayName);
                Console.WriteLine("Email: " + account.Email);
                Console.WriteLine();
            }
            return serviceAccounts;
        }
        // [END iam_list_service_accounts]

        // [START iam_rename_service_account]
        public static ServiceAccount RenameServiceAccount(string email,
            string newDisplayName)
        {
            // First, get a ServiceAccount using List() or Get()
            string resource = "projects/-/serviceAccounts/" + email;
            ServiceAccount serviceAccount = s_iam.Projects.ServiceAccounts
                .Get(resource).Execute();

            // Then you can update the display name
            serviceAccount.DisplayName = newDisplayName;
            serviceAccount = s_iam.Projects.ServiceAccounts.Update(
                serviceAccount, resource).Execute();

            Console.WriteLine($"Updated display name for {serviceAccount.Email} " +
                "to: " + serviceAccount.DisplayName);
            return serviceAccount;
        }
        // [END iam_rename_service_account]

        // [START iam_delete_service_account]
        public static void DeleteServiceAccount(string email)
        {
            string resource = "projects/-/serviceAccounts/" + email;
            s_iam.Projects.ServiceAccounts.Delete(resource).Execute();

            Console.WriteLine("Deleted service account: " + email);
        }
        // [END iam_delete_service_account]

        public static void Main(string[] args)
        {
            Init();
            Parser.Default.ParseArguments<
                CreateServiceAccountOptions,
                ListServiceAccountOptions,
                RenameServiceAccountOptions,
                DeleteServiceAccountOptions
                >(args).MapResult(
                (CreateServiceAccountOptions x) =>
                {
                    CreateServiceAccount(x.ProjectId, x.Name, x.DisplayName);
                    return 0;
                },
                (ListServiceAccountOptions x) =>
                {
                    ListServiceAccounts(x.ProjectId);
                    return 0;
                },
                (RenameServiceAccountOptions x) =>
                {
                    RenameServiceAccount(x.Email, x.DisplayName);
                    return 0;
                },
                (DeleteServiceAccountOptions x) =>
                {
                    DeleteServiceAccount(x.Email);
                    return 0;
                },
                error => 1);
        }

        public static void Init()
        {
            GoogleCredential credential = GoogleCredential.GetApplicationDefault()
                .CreateScoped(IamService.Scope.CloudPlatform);
            s_iam = new IamService(new IamService.Initializer
            {
                HttpClientInitializer = credential
            });
        }
    }
}