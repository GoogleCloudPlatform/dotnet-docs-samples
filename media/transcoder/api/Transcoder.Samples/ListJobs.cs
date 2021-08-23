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

// [START transcoder_list_jobs]

using Google.Cloud.Video.Transcoder.V1;
using Google.Api.Gax.ResourceNames;
using Google.Api.Gax;

public class ListJobsSample
{
    public string ListJobs(string projectId, string location)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parentLocation = new LocationName(projectId, location);

        // Call the API.
        PagedEnumerable<ListJobsResponse, Job> response = client.ListJobs(parentLocation);

        var result = "Jobs:\n";

        // Iterate over all response items, lazily performing RPCs as required.
        foreach (Job item in response)
        {
            // Add each job name to the result string.
            result += $"{item.Name}\n";
        }

        // Return the result.
        return result;
    }
}
// [END transcoder_list_jobs]