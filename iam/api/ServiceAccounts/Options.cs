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

namespace GoogleCloudSamples
{
    [Verb("create", HelpText = "Creates a service account.")]
    internal class CreateServiceAccountOptions
    {
        [Option("project-id", HelpText = "The project ID.", Required = true)]
        public string ProjectId { get; set; }

        [Option("name", HelpText = "The service account's name.", Required = true)]
        public string Name { get; set; }

        [Option("display-name", HelpText = "The service account's friendly display name.")]
        public string DisplayName { get; set; }
    }

    [Verb("list", HelpText = "Lists service accounts.")]
    internal class ListServiceAccountOptions
    {
        [Option("project-id", HelpText = "The project ID.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("rename", HelpText = "Updates a service account's display name.")]
    internal class RenameServiceAccountOptions
    {
        [Option("email", HelpText = "The service account email.", Required = true)]
        public string Email { get; set; }

        [Option("display-name", HelpText = "The service account's new friendly display name.", Required = true)]
        public string DisplayName { get; set; }
    }

    [Verb("delete", HelpText = "Deletes a service account.")]
    internal class DeleteServiceAccountOptions
    {
        [Option("email", HelpText = "The service account's email.", Required = true)]
        public string Email { get; set; }
    }
}