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

namespace GoogleCloudSamples
{
    [Verb("permissions", HelpText = "Lists valid permissions for a resource.")]
    internal class ViewPermissionsOptions
    {
        [Option("resource", HelpText = "", Required = true)]
        public string Resource { get; set; }
    }

    [Verb("get", HelpText = "Gets an existing roles.")]
    internal class GetRoleOptions
    {
        [Option("name", HelpText = "The name of the role.", Required = true)]
        public string Name { get; set; }
    }

    [Verb("list", HelpText = "Lists existing roles.")]
    internal class ListRolesOptions
    {
        [Option("project", HelpText = "The project to list roles for.")]
        public string Project { get; set; }
    }

    [Verb("create", HelpText = "Creates a custom role.")]
    internal class CreateRoleOptions
    {
        [Option("name", HelpText = "Name of the role.", Required = true)]
        public string Name { get; set; }

        [Option("project", HelpText = "Project for the role.", Required = true)]
        public string Project { get; set; }

        [Option("title", HelpText = "Title of the role.", Required = true)]
        public string Title { get; set; }

        [Option("description", HelpText = "Description of the role.",
            Required = true)]
        public string Description { get; set; }

        [Option("permissions", HelpText = "Permissions the role grants.",
            Required = true)]
        public List<String> Permissions { get; set; }

        [Option("stage", HelpText = "Stage of the role.", Required = true)]
        public string Stage { get; set; }
    }
    [Verb("edit", HelpText = "Modifies a custom role.")]
    internal class EditRoleOptions
    {
        [Option("name", HelpText = "Name of the role.")]
        public string Name { get; set; }

        [Option("project", HelpText = "Project for the role.")]
        public string Project { get; set; }

        [Option("title", HelpText = "Title of the role.")]
        public string Title { get; set; }

        [Option("description", HelpText = "Description of the role.")]
        public string Description { get; set; }

        [Option("permissions", HelpText = "Permissions the role grants.")]
        public List<String> Permissions { get; set; }

        [Option("stage", HelpText = "Stage of the role.")]
        public string Stage { get; set; }
    }

    [Verb("disable", HelpText = "Disables a custom role.")]
    internal class DisableRoleOptions
    {
        [Option("name", HelpText = "Name of the role.")]
        public string Name { get; set; }

        [Option("project", HelpText = "Project for the role.")]
        public string Project { get; set; }
    }

    [Verb("delete", HelpText = "Deletes a custom role.")]
    internal class DeleteRoleOptions
    {
        [Option("name", HelpText = "Name of the role.")]
        public string Name { get; set; }

        [Option("project", HelpText = "Project for the role.")]
        public string Project { get; set; }
    }

    [Verb("undelete", HelpText = "Undeletes a custom role.")]
    internal class UndeleteRoleOptions
    {
        [Option("name", HelpText = "Name of the role.")]
        public string Name { get; set; }

        [Option("project", HelpText = "Project for the role.")]
        public string Project { get; set; }
    }
}