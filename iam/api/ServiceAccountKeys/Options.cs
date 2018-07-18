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
    [Verb("create", HelpText = "Creates a service account key.")]
    internal class CreateKeyOptions
    {
        [Option("service-account-email", HelpText = "The service account's email.", Required = true)]
        public string ServiceAccountEmail { get; set; }
    }

    [Verb("list", HelpText = "Lists the keys for a service account.")]
    internal class ListKeyOptions
    {
        [Option("service-account-email", HelpText = "The service account's email.", Required = true)]
        public string ServiceAccountEmail { get; set; }
    }

    [Verb("delete", HelpText = "Deletes a service account key.")]
    internal class DeleteKeyOptions
    {
        [Option("full-key-name", HelpText = "The full resource name of the key.", Required = true)]
        public string FullKeyName { get; set; }
    }
}