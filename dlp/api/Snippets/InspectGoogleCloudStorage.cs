// Copyright 2020 Google Inc.
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

// [START dlp_inspect_gcs]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class InspectGoogleCloudStorage
{
    public static DlpJob InspectGCS(
        string projectId,
        Likelihood minLikelihood,
        int maxFindings,
        bool includeQuote,
        IEnumerable<InfoType> infoTypes,
        IEnumerable<CustomInfoType> customInfoTypes,
        string bucketName,
        string topicId,
        string subscriptionId)
    {
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = new StorageConfig
            {
                CloudStorageOptions = new CloudStorageOptions
                {
                    FileSet = new CloudStorageOptions.Types.FileSet { Url = $"gs://{bucketName}/*.txt" },
                    BytesLimitPerFile = 1073741824
                },
            },
            InspectConfig = new InspectConfig
            {
                InfoTypes = { infoTypes },
                CustomInfoTypes = { customInfoTypes },
                ExcludeInfoTypes = false,
                IncludeQuote = includeQuote,
                Limits = new FindingLimits
                {
                    MaxFindingsPerRequest = maxFindings
                },
                MinLikelihood = minLikelihood
            },
            Actions =
                {
                    new Google.Cloud.Dlp.V2.Action
                    {
                        // Send results to Pub/Sub topic
                        PubSub = new Google.Cloud.Dlp.V2.Action.Types.PublishToPubSub
                        {
                            Topic = topicId,
                        }
                    }
                }
        };

        // Issue Create Dlp Job Request
        var client = DlpServiceClient.Create();
        var request = new CreateDlpJobRequest
        {
            InspectJob = inspectJob,
            Parent = new LocationName(projectId, "global").ToString(),
        };

        // We need created job name
        var dlpJob = client.CreateDlpJob(request);

        // Get a pub/sub subscription and listen for DLP results
        var fireEvent = new ManualResetEventSlim();

        var subscriptionName = new SubscriptionName(projectId, subscriptionId);
        var subscriber = SubscriberClient.CreateAsync(subscriptionName).Result;
        subscriber.StartAsync(
            (pubSubMessage, cancellationToken) =>
            {
                // Given a message that we receive on this subscription, we should either acknowledge or decline it
                if (pubSubMessage.Attributes["DlpJobName"] == dlpJob.Name)
                {
                    fireEvent.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }

                return Task.FromResult(SubscriberClient.Reply.Nack);
            });

        // We block here until receiving a signal from a separate thread that is waiting on a message indicating receiving a result of Dlp job
        if (fireEvent.Wait(TimeSpan.FromMinutes(1)))
        {
            // Stop the thread that is listening to messages as a result of StartAsync call earlier
            subscriber.StopAsync(CancellationToken.None).Wait();

            // Now we can inspect full job results
            var job = client.GetDlpJob(new GetDlpJobRequest { DlpJobName = new DlpJobName(projectId, dlpJob.Name) });

            // Inspect Job details
            Console.WriteLine($"Processed bytes: {job.InspectDetails.Result.ProcessedBytes}");
            Console.WriteLine($"Total estimated bytes: {job.InspectDetails.Result.TotalEstimatedBytes}");
            var stats = job.InspectDetails.Result.InfoTypeStats;
            Console.WriteLine("Found stats:");
            foreach (var stat in stats)
            {
                Console.WriteLine($"{stat.InfoType.Name}");
            }

            return job;
        }

        throw new InvalidOperationException("The wait failed on timeout");
    }
}
// [END dlp_inspect_gcs]
