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

// [START compute_usage_report_disable]

using Google.Cloud.Compute.V1;
using System.Threading.Tasks;

public class DisableUsageExportBucketAsyncSample
{
    public async Task DisableUsageExportBucketAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id")
    {
        // Create a default instance of UsageExportLocation.
        // Do not set any properties.
        UsageExportLocation exportLocation = new UsageExportLocation();

        // Initialize the client that will be used to send project-related requests.
        // You should reuse the same client for multiple requests.
        ProjectsClient client = await ProjectsClient.CreateAsync();

        // Setting the default instance will disable the usage report export bucket setting.
        var operation = await client.SetUsageExportBucketAsync(projectId, exportLocation);
        // Wait for the operation to complete using client-side polling.
        await operation.PollUntilCompletedAsync();
    }
}

// [END compute_usage_report_disable]
