// Copyright 2023 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START dlp_inspect_send_data_to_hybrid_job_trigger]

using System;
using Google.Api.Gax.ResourceNames;
using Google.Api.Gax;
using Google.Cloud.Dlp.V2;
using Grpc.Core;

public class SendDataToTheHybridJobTrigger
{
    public static DlpJob SendToHybridJobTrigger(
       string projectId,
       string jobTriggerId,
       string text = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the hybrid finding details which will be used as metadata with the content.
        // Refer to this for more information: https://cloud.google.com/dlp/docs/reference/rpc/google.privacy.dlp.v2#google.privacy.dlp.v2.Container
        var findingDetails = new HybridFindingDetails
        {
            ContainerDetails = new Container
            {
                FullPath = "10.0.0.2:logs1:aap1",
                RelativePath = "app1",
                RootPath = "10.0.0.2:logs1",
                Type = "System Logs"
            }
        };

        // Construct the hybrid content item using the finding details and text to be inspected.
        var hybridContentItem = new HybridContentItem
        {
            Item = new ContentItem { Value = text ?? "My email is ariel@example.org and name is Ariel." },
            FindingDetails = findingDetails
        };

        var jobTriggerName = new JobTriggerName(projectId, jobTriggerId);

        // Construct the request to activate the Job Trigger.
        var activate = new ActivateJobTriggerRequest
        {
            JobTriggerName = jobTriggerName
        };

        DlpJob triggerJob = null;

        try
        {
            // Call the API to activate the trigger.
            triggerJob = dlp.ActivateJobTrigger(activate);
        }
        catch (RpcException)
        {
            ListDlpJobsRequest listJobsRequest = new ListDlpJobsRequest
            {
                ParentAsLocationName = new LocationName(projectId, "global"),
                Filter = $"trigger_name={jobTriggerName}"
            };

            PagedEnumerable<ListDlpJobsResponse, DlpJob> res = dlp.ListDlpJobs(listJobsRequest);
            foreach (DlpJob j in res)
            {
                triggerJob = j;
            }
        }

        // Construct the request using hybrid content item.
        var request = new HybridInspectJobTriggerRequest
        {
            HybridItem = hybridContentItem,
            JobTriggerName = jobTriggerName
        };

        // Call the API.
        HybridInspectResponse _ = dlp.HybridInspectJobTrigger(request);

        Console.WriteLine($"Hybrid job created successfully. Job name: {triggerJob.Name}");

        return triggerJob;
    }
}

// [END dlp_inspect_send_data_to_hybrid_job_trigger]
