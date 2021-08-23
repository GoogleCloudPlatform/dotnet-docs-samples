﻿/*
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

// [START transcoder_create_job_from_preset]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Video.Transcoder.V1;

public class CreateJobFromPresetSample
{
    public string CreateJobFromPreset(
        string projectId, string location, string inputUri, string outputUri, string preset)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the parent location name.
        LocationName parent = new LocationName(projectId, location);

        // Build the job config.
        Job job = new Job();
        job.InputUri = inputUri;
        job.OutputUri = outputUri;
        job.TemplateId = preset;

        // Call the API.
        Job response = client.CreateJob(parent, job);

        // Return the result.
        return "Job: " + response.JobName;
    }
}
// [END transcoder_create_job_from_preset]