// Copyright 2020 Google Inc.
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

// [START dlp_list_jobs]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

public class JobsList
{
    public static PagedEnumerable<ListDlpJobsResponse, DlpJob> ListDlpJobs(string projectId, string filter, DlpJobType jobType)
    {
        var dlp = DlpServiceClient.Create();

        var response = dlp.ListDlpJobs(new ListDlpJobsRequest
        {
            Parent = new LocationName(projectId, "global").ToString(),
            Filter = filter,
            Type = jobType
        });

        // Uncomment to print jobs
        // foreach (var job in response)
        // {
        //     Console.WriteLine($"Job: {job.Name} status: {job.State}");
        // }

        return response;
    }
}

// [END dlp_list_jobs]
