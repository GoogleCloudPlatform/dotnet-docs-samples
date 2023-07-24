/*
 * Copyright 2023 Google LLC
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

// [START transcoder_create_job_from_preset_batch_mode]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobFromPresetBatchModeSample
{
    public Job CreateJobFromPresetBatchMode(
        string projectId, string location, string inputUri, string outputUri, string preset)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parent = new LocationName(projectId, location);

        // Build the job.
        Job newJob = new Job
        {
            InputUri = inputUri,
            OutputUri = outputUri,
            TemplateId = preset,
            Mode = Job.Types.ProcessingMode.Batch,
            BatchModePriority = 10
        };

        // Call the API.
        Job job = client.CreateJob(parent, newJob);

        // Return the result.
        return job;
    }
}
// [END transcoder_create_job_from_preset_batch_mode]
