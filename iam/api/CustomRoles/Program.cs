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
    public static class CustomRoles
    {
        private static IamService s_service;

        // [START iam_query_testable_permissions]
        public static IList<Permission> QueryTestablePermissions(
            string resource)
        {
            var request = new QueryTestablePermissionsRequest
            {
                FullResourceName = resource
            };
            var permissions = s_service.Permissions.QueryTestablePermissions(
                request).Execute().Permissions;

            foreach (var p in permissions)
            {
                Console.WriteLine(p.Name);
            }
            return permissions;
        }
        // [END iam_query_testable_permissions]

        // [START iam_get_role]
        public static Role GetRole(string name)
        {
            Role role = s_service.Roles.Get(name).Execute();
            Console.WriteLine(role.Name);
            Console.WriteLine(String.Join(", ", role.IncludedPermissions));
            return role;
        }
        // [END iam_get_role]

        // [START iam_list_roles]
        public static IList<Role> ListRoles(string projectId)
        {
            ListRolesResponse response = s_service.Projects.Roles
                .List("projects/" + projectId).Execute();
            foreach (var role in response?.Roles)
            {
                Console.WriteLine(role.Name);
            }
            return response.Roles;
        }
        // [END iam_list_roles]

        // [START iam_create_role]
        public static Role CreateRole(string name, string project,
            string title, string description, IList<string> permissions,
            string stage)
        {
            Role role = new Role
            {
                Title = title,
                Description = description,
                IncludedPermissions = permissions,
                Stage = stage
            };
            var request = new CreateRoleRequest
            {
                Role = role,
                RoleId = name
            };

            role = s_service.Projects.Roles.Create(request,
                "projects/" + project).Execute();

            Console.WriteLine("Created role: " + role.Name);
            return role;
        }
        // [END iam_create_role]

        // [START iam_edit_role]
        public static Role EditRole(string name, string project,
            string newTitle, string newDescription,
            IList<string> newPermissions, string newStage)
        {
            string resource = $"projects/{project}/roles/{name}";

            Role role = s_service.Projects.Roles.Get(resource).Execute();

            role.Title = newTitle;
            role.Description = newDescription;
            role.IncludedPermissions = newPermissions;
            role.Stage = newStage;

            role = s_service.Projects.Roles.Patch(role, resource).Execute();
            Console.WriteLine("Updated role: " + role.Name);
            return role;
        }
        // [END iam_edit_role]

        // [START iam_disable_role]
        public static Role DisableRole(string name, string project)
        {
            string resource = $"projects/{project}/roles/{name}";

            Role role = s_service.Projects.Roles.Get(resource).Execute();
            role.Stage = "DISABLED";

            role = s_service.Projects.Roles.Patch(new Role
            {
            }, $"projects/{project}/roles/{name}").Execute();

            Console.WriteLine("Disabled role: " + role.Name);
            return role;
        }
        // [END iam_disable_role]

        // [START iam_delete_role]
        public static void DeleteRole(string name, string project)
        {
            s_service.Projects.Roles.Delete(
                $"projects/{project}/roles/{name}").Execute();
            Console.WriteLine("Deleted role: " + name);
        }
        // [END iam_delete_role]

        // [START iam_undelete_role]
        public static Role UndeleteRole(string name, string project)
        {
            string resource = $"projects/{project}/roles/{name}";
            Role role = s_service.Projects.Roles.Undelete(
                new UndeleteRoleRequest(), resource).Execute();

            Console.WriteLine("Undeleted role: " + role.Name);
            return role;
        }
        // [END iam_undelete_role]

        public static void Main(string[] args)
        {
            Init();

            CommandLine.Parser.Default.ParseArguments<
                ViewPermissionsOptions,
                GetRoleOptions,
                ListRolesOptions,
                CreateRoleOptions,
                EditRoleOptions,
                DisableRoleOptions,
                DeleteRoleOptions,
                UndeleteRoleOptions
                >(args).MapResult(
                (ViewPermissionsOptions x) =>
                {
                    QueryTestablePermissions(x.Resource);
                    return 0;
                },
                (GetRoleOptions x) =>
                {
                    GetRole(x.Name);
                    return 0;
                },
                (ListRolesOptions x) =>
                {
                    ListRoles(x.Project);
                    return 0;
                },
                (CreateRoleOptions x) =>
                {
                    CreateRole(x.Name, x.Project, x.Title, x.Description,
                        x.Permissions, x.Stage);
                    return 0;
                },
                (EditRoleOptions x) =>
                {
                    EditRole(x.Name, x.Project, x.Title, x.Description,
                        x.Permissions, x.Stage);
                    return 0;
                },
                (DisableRoleOptions x) =>
                {
                    DisableRole(x.Name, x.Project);
                    return 0;
                },
                (DeleteRoleOptions x) =>
                {
                    DeleteRole(x.Name, x.Project);
                    return 0;
                },
                (UndeleteRoleOptions x) =>
                {
                    UndeleteRole(x.Name, x.Project);
                    return 0;
                },
                error => 1);
        }

        public static void Init()
        {
            GoogleCredential credential = GoogleCredential.GetApplicationDefault()
                .CreateScoped(IamService.Scope.CloudPlatform);
            s_service = new IamService(new IamService.Initializer
            {
                HttpClientInitializer = credential
            });
        }
    }
}
