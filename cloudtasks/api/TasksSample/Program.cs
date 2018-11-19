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
using CommandLine;
using Google.Cloud.Tasks.V2Beta2;

namespace GoogleCloudSamples
{
    [Verb("createTask", HelpText = "Creates a Cloud Task.")]
    class CreateTaskOptions
    {
        [Option('i', "projectId", HelpText = "Project ID of the queue to add the task to.", Required = true)]
        public string ProjectId { get; set; }

        [Option('l', "location", HelpText = "Location of the queue to add the task to.", Required = true)]
        public string Location { get; set; }

        [Option('q', "queue", HelpText = "Location of the queue to add the task to.", Required = true)]
        public string Queue { get; set; }

        [Option('d', "payload", HelpText = "(Optional) Payload to attach to the push queue.", Default = "")]
        public string Payload { get; set; }

        [Option('s', "inSeconds",
            HelpText = "(Optional) The number of seconds from now to schedule task attempt.", Default = 0)]
        public int InSeconds { get; set; }
    }

    public partial class Tasks
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid number of arguments supplied");
                Environment.Exit(-1);
            }
            
            Parser.Default.ParseArguments<
            CreateTaskOptions>(args).MapResult(
            (CreateTaskOptions opts) => Samples.CreateTask(
                opts.ProjectId,
                opts.Location,
                opts.Queue,
                opts.Payload,
                opts.InSeconds),
            errs => 1);
        }
    }
}
