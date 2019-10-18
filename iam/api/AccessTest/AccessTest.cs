// Copyright 2019 Google Inc.
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
using Google.Apis.Auth.OAuth2;
using Xunit;

using System.Collections.Generic;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

namespace GoogleCloudSamples
{
    public class AccessTest
    {
        private readonly string _project;
        private readonly string _member1;
        private readonly string _member2;
        private readonly string _member3;
        public AccessTest()
        {
            _project = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _member1 = "user:yaraarryn.677500@gmail.com";
            _member2 = "user:obaraclegane.724498@gmail.com";
            _member3 = "daeneryssnow.827847@gmail.com";
        }

        public IamService InitializeService()
        {
            var credential = GoogleCredential.GetApplicationDefault()
                .CreateScoped(IamService.Scope.CloudPlatform);
            var service = new IamService(new IamService.Initializer
            {
                HttpClientInitializer = credential
            });

            return service;
        }

        public Role CreateCustomRole(IamService service)
        {
            var role = new Role
            {
                Title = "C# Test Custom Role",
                Description = "Role for AccessTest",
                IncludedPermissions = new List<string> { "iam.roles.get" },
                Stage = "GA"
            };

            var request = new CreateRoleRequest
            {
                Role = role,
                RoleId = "csharpTestCustomRole" + new Random().Next()
            };

            role = service.Projects.Roles.Create(request, "projects/" + _project).Execute();
            return role;
        }

        public String ParseRoleName(Role role)
        {
            var roleNameComponents = role.Name.Split('/');
            var roleNameShort = roleNameComponents[2] + "/" + roleNameComponents[3];
            return roleNameShort;
        }

        [Fact]
        public void TestAccess()
        {
            var service = InitializeService();

            var role1 = CreateCustomRole(service);
            var role1NameShort = ParseRoleName(role1);
            try
            {
                var role2 = CreateCustomRole(service);
                var role2NameShort = ParseRoleName(role2);
                try
                {
                    // Test GetPolicy
                    var policy = AccessManager.GetPolicy(_project);

                    // Test AddBinding by adding _member1 to role1
                    policy = AccessManager.AddBinding(policy, role1NameShort, _member1);

                    // Test AddMember by adding _member2 to role1
                    policy = AccessManager.AddMember(policy, role1NameShort, _member2);

                    // Test RemoveMember where role binding doesn't exist (_member1 from role2)
                    policy = AccessManager.RemoveMember(policy, role2NameShort, _member1);

                    // Test RemoveMember where member doesn't exist (_member3 from role1)
                    policy = AccessManager.RemoveMember(policy, role1NameShort, _member3);

                    // Test RemoveMember by removing _member1 from role1
                    policy = AccessManager.RemoveMember(policy, role1NameShort, _member1);

                    // Test RemoveMember when removing last member from binding (_member2 from role1)
                    policy = AccessManager.RemoveMember(policy, role1NameShort, _member2);

                    // Test SetPolicy
                    policy = AccessManager.SetPolicy(_project, policy);
                }
                finally
                {
                    // Delete custom roles
                    service.Projects.Roles.Delete(role2.Name).Execute();
                }
            }
            finally
            {
                service.Projects.Roles.Delete(role1.Name).Execute();
            }
        }

        [Fact]
        public void TestPermissions()
        {
            var permissions = AccessManager.TestIamPermissions(_project);
        }
    }
}
