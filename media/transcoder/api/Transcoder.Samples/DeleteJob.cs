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

// [START transcoder_delete_job]

using Google.Cloud.Video.Transcoder.V1;

public class DeleteJobSample
{
    public void DeleteJob(string projectId, string location, string jobId)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the job name.
        JobName jobName = JobName.FromProjectLocationJob(projectId, location, jobId);

        // Call the API.
        client.DeleteJob(jobName);
    }
}
// [END transcoder_delete_job]