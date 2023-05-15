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

// [START dlp_inspect_gcs_with_sampling]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class InspectStorageWithSampling
{
    public static async Task<DlpJob> InspectAsync(
        string projectId,
        string gcsUri,
        string topicId,
        string subId,
        Likelihood minLikelihood = Likelihood.Possible,
        IEnumerable<InfoType> infoTypes = null)
    {

        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct Storage config by specifying the GCS file to be inspected
        // and sample method.
        var storageConfig = new StorageConfig
        {
            CloudStorageOptions = new CloudStorageOptions
            {
                FileSet = new CloudStorageOptions.Types.FileSet
                {
                    Url = gcsUri
                },
                BytesLimitPerFile = 200,
                FileTypes = { new FileType[] { FileType.Csv } },
                FilesLimitPercent = 90,
                SampleMethod = CloudStorageOptions.Types.SampleMethod.RandomStart
            }
        };

        // Construct the Inspect Config and specify the type of info the inspection
        // will look for.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                infoTypes ?? new InfoType[] { new InfoType { Name = "PERSON_NAME" } }
            },
            IncludeQuote = true,
            MinLikelihood = minLikelihood
        };

        // Construct the pubsub action.
        var actions = new Action[]
        {
            new Action
            {
                PubSub = new Action.Types.PublishToPubSub
                {
                    Topic = $"projects/{projectId}/topics/{topicId}"
                }
            }
        };

        // Construct the inspect job config using above created objects.
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = storageConfig,
            InspectConfig = inspectConfig,
            Actions = { actions }
        };

        // Issue Create Dlp Job Request
        var request = new CreateDlpJobRequest
        {
            InspectJob = inspectJob,
            ParentAsLocationName = new LocationName(projectId, "global"),
        };

        // We keep the name of the job that we just created.
        var dlpJob = dlp.CreateDlpJob(request);
        var jobName = dlpJob.Name;

        // Listen to pub/sub for the job
        var subscriptionName = new SubscriptionName(projectId, subId);
        var subscriber = await SubscriberClient.CreateAsync(
            subscriptionName);

        await subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            if (message.Attributes["DlpJobName"] == jobName)
            {
                subscriber.StopAsync(cancel);
                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            else
            {
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
        });

        // Get the latest state of the job from the service
        var resultJob = dlp.GetDlpJob(new GetDlpJobRequest
        {
            DlpJobName = DlpJobName.Parse(jobName)
        });

        // Parse the response and process results.
        System.Console.WriteLine($"Job status: {resultJob.State}");
        System.Console.WriteLine($"Job Name: {resultJob.Name}");

        var result = resultJob.InspectDetails.Result;
        foreach (var infoType in result.InfoTypeStats)
        {
            System.Console.WriteLine($"Info Type: {infoType.InfoType.Name}");
            System.Console.WriteLine($"Count: {infoType.Count}");
        }
        return resultJob;
    }
}

// [END dlp_inspect_gcs_with_sampling]
