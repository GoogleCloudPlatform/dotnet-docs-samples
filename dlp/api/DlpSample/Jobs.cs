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
using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GoogleCloudSamples
{
    [Verb("listJobs", HelpText = "List Data Loss Prevention API jobs corresponding to a given filter.")]
    abstract class ListJobsOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(0, HelpText = "The filter expression to use. For more information and filter syntax, see https://cloud.google.com/dlp/docs/reference/rest/v2/projects.dlpJobs/list", Required = true)]
        public string Filter { get; set; }

        [Value(0, HelpText = "The type of job to list. (either 'InspectJob' or 'RiskAnalysisJob')", Required = false, Default = "InspectJob")]
        public string JobType { get; set; }
    }

    [Verb("deleteJob", HelpText = "Delete results of a Data Loss Prevention API job.")]
    abstract class DeleteJobOptions
    {
        [Value(0, HelpText = "The full name of the job whose results should be deleted.", Required = true)]
        public string JobName { get; set; }
    }

    public class Jobs
    {
        static object ListJobs(ListJobsOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            var response = dlp.ListDlpJobs(new ListDlpJobsRequest
            {
                Parent = $"projects/{opts.ProjectId}",
                Filter = opts.Filter,
                Type = (DlpJobType) Enum.Parse(typeof(DlpJobType), opts.JobType)
            });

            foreach (var job in response)
            {
                Console.WriteLine($"Job: {job.Name} status: {job.State}");
            }

            return 0;
        }

        static object DeleteJob(DeleteJobOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            dlp.DeleteDlpJob(new DeleteDlpJobRequest
            {
                Name = opts.JobName
            });

            Console.WriteLine($"Successfully deleted job {opts.JobName}.");
            return 0;
        }
    }
}