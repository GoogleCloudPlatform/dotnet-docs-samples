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

using CommandLine;

public partial class CustomRoles
{
    public static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<
            ViewPermissionsOptions,
            ViewGrantableRolesOptions,
            GetRoleOptions,
            ListRolesOptions,
            CreateRoleOptions,
            EditRoleOptions,
            DeleteRoleOptions,
            UndeleteRoleOptions
            >(args).MapResult(
            (ViewPermissionsOptions x) =>
            {
                QueryTestablePermissions(x.Resource);
                return 0;
            },
            (ViewGrantableRolesOptions x) =>
            {
                ViewGrantableRoles(x.Resource);
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
}
