// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class DialogflowTest
    {
        public readonly string ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public readonly string SessionId = TestUtil.RandomName();

        public ConsoleOutput Output { get; set; }
        public string Stdout => Output.Stdout;
        public int ExitCode => Output.ExitCode;

        // Multiple tests depend on existing EntityTypes.
        //
        // This helper method creates an EntityType via `entity-types:create`
        // and returns the EntityType's ID.
        public string CreateEntityType(string displayName = null, string kindName = "Map")
        {
            if (string.IsNullOrEmpty(displayName))
                displayName = TestUtil.RandomName();
            Run("entity-types:create", displayName, kindName);
            var outputPattern = new Regex(
                $"Created EntityType: projects/{ProjectId}/agent/entityTypes/(?<entityTypeId>.*)"
            );
            return outputPattern.Match(Stdout).Groups["entityTypeId"].Value;
        }

        public readonly CommandLineRunner _dialogflow = new CommandLineRunner()
        {
            Main = DialogflowSamples.Main,
            Command = "Dialogflow"
        };

        // Run command and return output.
        // Project ID argument is always set.
        // Session ID argument available as a parameter.
        // Sets helper properties to last console output.
        public ConsoleOutput Run(string command, params object[] args)
        {
            var arguments = args.Select((arg) => arg.ToString()).ToList();
            arguments.Insert(0, command);
            arguments.AddRange(new[] { "--projectId", ProjectId });

            Output = _dialogflow.Run(arguments.ToArray());

            return Output;
        }

        public ConsoleOutput RunWithSessionId(string command, params object[] args)
        {
            var arguments = args.ToList();
            arguments.AddRange(new[] { "--sessionId", SessionId });
            return Run(command, arguments.ToArray());
        }
    }
}
