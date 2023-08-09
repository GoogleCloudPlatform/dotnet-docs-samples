// Copyright 2023 Google Inc.
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

// [START dlp_update_trigger]

using Google.Cloud.Dlp.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class TriggersUpdate
{
    public static JobTrigger UpdateJob(
        string projectId,
        string triggerId,
        IEnumerable<InfoType> infoTypes = null,
        Likelihood minLikelihood = Likelihood.Likely)
    {
        // Instantiate the client.
        var dlp = DlpServiceClient.Create();

        // Construct the update job trigger request object by providing the trigger name,
        // job trigger object which will specify the type of info to be inspected and
        // update mask object which specifies the field to be updated.
        // Refer to https://cloud.google.com/dlp/docs/reference/rest/v2/Container for specifying the paths in container object.
        var request = new UpdateJobTriggerRequest
        {
            JobTriggerName = new JobTriggerName(projectId, triggerId),
            JobTrigger = new JobTrigger
            {
                InspectJob = new InspectJobConfig
                {
                    InspectConfig = new InspectConfig
                    {
                        InfoTypes =
                        {
                            infoTypes ?? new InfoType[]
                            {
                                new InfoType { Name = "US_INDIVIDUAL_TAXPAYER_IDENTIFICATION_NUMBER" }
                            }
                        },
                        MinLikelihood = minLikelihood
                    }
                }
            },
            // Specify fields of the jobTrigger resource to be updated when the job trigger is modified.
            // Refer https://protobuf.dev/reference/protobuf/google.protobuf/#field-mask for constructing the field mask paths.
            UpdateMask = new FieldMask
            {
                Paths =
                {
                    "inspect_job.inspect_config.info_types",
                    "inspect_job.inspect_config.min_likelihood"
                }
            }
        };

        // Call the API.
        JobTrigger response = dlp.UpdateJobTrigger(request);

        // Inspect the result.
        Console.WriteLine($"Job Trigger Name: {response.Name}");
        Console.WriteLine($"InfoType updated: {response.InspectJob.InspectConfig.InfoTypes[0]}");
        Console.WriteLine($"Likelihood updated: {response.InspectJob.InspectConfig.MinLikelihood}");
        return response;
    }
}

// [END dlp_update_trigger]
