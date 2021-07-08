/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START compute_usage_report_get]

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public class GetUsageExportBucketAsyncSample
{
    public async Task<Project> GetUsageExportBucketAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id")
    {
        // Initialize the client that will be used to send project-related requests.
        // You should reuse the same client for multiple requests.
        ProjectsClient client = await ProjectsClient.CreateAsync();

        Project project = await client.GetAsync(projectId);

        if (project.UsageExportLocation is null)
        {
            Console.WriteLine("This project has no usage report export location configured.");
        }
        else if (string.IsNullOrEmpty(project.UsageExportLocation.ReportNamePrefix))
        {
            Console.WriteLine(
                $"Setting {nameof(UsageExportLocation.ReportNamePrefix)} " +
                $"to null or empty values causes the report to have the default prefix of `usage_gce`.");
        }

        return project;
    }
}

// [END compute_usage_report_get]
