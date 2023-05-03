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

// [START dlp_inspect_bigquery_with_sampling]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class InspectBigQueryWithSampling
{
    public static async Task<DlpJob> InspectAsync(
        string projectId,
        int maxFindings,
        bool includeQuote,
        string topicId,
        string subId,
        Likelihood minLikelihood = Likelihood.Possible,
        IEnumerable<FieldId> identifyingFields = null,
        IEnumerable<InfoType> infoTypes = null)
    {

        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct Storage config.
        var storageConfig = new StorageConfig
        {
            BigQueryOptions = new BigQueryOptions
            {
                TableReference = new BigQueryTable
                {
                    ProjectId = "bigquery-public-data",
                    DatasetId = "usa_names",
                    TableId = "usa_1910_current",
                },
                IdentifyingFields =
                {
                    identifyingFields ?? new FieldId[] { new FieldId { Name = "name" } }
                },
                RowsLimit = 100,
                SampleMethod = BigQueryOptions.Types.SampleMethod.RandomStart
            }
        };

        // Construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes = { infoTypes ?? new InfoType[] { new InfoType { Name = "PERSON_NAME" } } },
            Limits = new FindingLimits
            {
                MaxFindingsPerRequest = maxFindings,
            },
            IncludeQuote = includeQuote,
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

        // Construct the inspect job config using the actions.
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = storageConfig,
            InspectConfig = inspectConfig,
            Actions = { actions }
        };

        // Issue Create Dlp Job Request.
        var request = new CreateDlpJobRequest
        {
            InspectJob = inspectJob,
            ParentAsLocationName = new LocationName(projectId, "global"),
        };

        // We keep the name of the job that we just created.
        var dlpJob = dlp.CreateDlpJob(request);
        var jobName = dlpJob.Name;

        // Listen to pub/sub for the job.
        var subscriptionName = new SubscriptionName(projectId, subId);
        var subscriber = await SubscriberClient.CreateAsync(
            subscriptionName);

        // SimpleSubscriber runs your message handle function on multiple threads to maximize throughput.
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
        
        // Get the latest state of the job from the service.
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

// [END dlp_inspect_bigquery_with_sampling]
