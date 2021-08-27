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

// [START transcoder_get_job_template]

using Google.Cloud.Video.Transcoder.V1;

public class GetJobTemplateSample
{
    public JobTemplate GetJobTemplate(string projectId, string location, string templateId)
    {
        // Create the client.
        TranscoderServiceClient client = TranscoderServiceClient.Create();

        // Build the job template name.
        JobTemplateName name = JobTemplateName.FromProjectLocationJobTemplate(projectId, location, templateId);

        // Call the API.
        JobTemplate jobTemplate = client.GetJobTemplate(name);

        // Return the result.
        return jobTemplate;
    }
}
// [END transcoder_get_job_template]