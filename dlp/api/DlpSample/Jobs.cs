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
    /// <summary>
    /// This class contains examples of how to list and delete DLP jobs
    /// For more information, see https://cloud-dot-devsite.googleplex.com/dlp/docs/reference/rest/v2/projects.dlpJobs
    /// </summary>
    public static class Jobs
    {
        // [START dlp_list_jobs]
        public static object ListJobs(string ProjectId, string Filter, string JobType)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            var response = dlp.ListDlpJobs(new ListDlpJobsRequest
            {
                ParentAsProjectName = new ProjectName(ProjectId),
                Filter = Filter,
                Type = (DlpJobType) Enum.Parse(typeof(DlpJobType), JobType)
            });

            foreach (var job in response)
            {
                Console.WriteLine($"Job: {job.Name} status: {job.State}");
            }

            return 0;
        }
        // [END dlp_list_jobs]

        // [START dlp_delete_job]
        public static object DeleteJob(string JobName)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            dlp.DeleteDlpJob(new DeleteDlpJobRequest
            {
                Name = JobName
            });

            Console.WriteLine($"Successfully deleted job {JobName}.");
            return 0;
        }
        // [END dlp_delete_job]
    }
}