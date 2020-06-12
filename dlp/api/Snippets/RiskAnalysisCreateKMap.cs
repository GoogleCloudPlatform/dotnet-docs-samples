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

// [START dlp_k_map]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Dlp.V2.Action.Types;
using static Google.Cloud.Dlp.V2.PrivacyMetric.Types;
using static Google.Cloud.Dlp.V2.PrivacyMetric.Types.KMapEstimationConfig.Types;

public class RiskAnalysisCreateKMap
{
    public static object KMap(
        string callingProjectId,
        string tableProjectId,
        string datasetId,
        string tableId,
        string topicId,
        string subscriptionId,
        IEnumerable<FieldId> quasiIds,
        IEnumerable<InfoType> infoTypes,
        string regionCode)
    {
        var dlp = DlpServiceClient.Create();

        // Construct + submit the job
        var kmapEstimationConfig = new KMapEstimationConfig
        {
            QuasiIds =
                {
                    quasiIds.Zip(
                        infoTypes,
                        (Field, InfoType) => new TaggedField
                        {
                            Field = Field,
                            InfoType = InfoType
                        }
                    )
                },
            RegionCode = regionCode
        };

        var config = new RiskAnalysisJobConfig()
        {
            PrivacyMetric = new PrivacyMetric
            {
                KMapEstimationConfig = kmapEstimationConfig
            },
            SourceTable = new BigQueryTable
            {
                ProjectId = tableProjectId,
                DatasetId = datasetId,
                TableId = tableId
            },
            Actions =
            {
                new Google.Cloud.Dlp.V2.Action
                {
                    PubSub = new PublishToPubSub
                    {
                        Topic = $"projects/{callingProjectId}/topics/{topicId}"
                    }
                }
            }
        };

        var submittedJob = dlp.CreateDlpJob(
            new CreateDlpJobRequest
            {
                ParentAsProjectName = new ProjectName(callingProjectId),
                RiskJob = config
            });

        // Listen to pub/sub for the job
        var subscriptionName = new SubscriptionName(
            callingProjectId,
            subscriptionId);
        var subscriber = SubscriberClient.CreateAsync(
            subscriptionName).Result;

        // SimpleSubscriber runs your message handle function on multiple
        // threads to maximize throughput.
        var done = new ManualResetEventSlim(false);
        subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
        {
            if (message.Attributes["DlpJobName"] == submittedJob.Name)
            {
                Thread.Sleep(500); // Wait for DLP API results to become consistent
                done.Set();
                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            else
            {
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
        });

        done.Wait(TimeSpan.FromMinutes(10)); // 10 minute timeout; may not work for large jobs
        subscriber.StopAsync(CancellationToken.None).Wait();

        // Process results
        var resultJob = dlp.GetDlpJob(new GetDlpJobRequest
        {
            DlpJobName = DlpJobName.Parse(submittedJob.Name)
        });

        var result = resultJob.RiskDetails.KMapEstimationResult;

        for (var histogramIdx = 0; histogramIdx < result.KMapEstimationHistogram.Count; histogramIdx++)
        {
            var histogramValue = result.KMapEstimationHistogram[histogramIdx];
            Console.WriteLine($"Bucket {histogramIdx}");
            Console.WriteLine($"  Anonymity range: [{histogramValue.MinAnonymity}, {histogramValue.MaxAnonymity}].");
            Console.WriteLine($"  Size: {histogramValue.BucketSize}");

            foreach (var datapoint in histogramValue.BucketValues)
            {
                // 'UnpackValue(x)' is a prettier version of 'x.toString()'
                Console.WriteLine($"    Values: [{String.Join(',', datapoint.QuasiIdsValues.Select(x => UnpackValue(x)))}]");
                Console.WriteLine($"    Estimated k-map anonymity: {datapoint.EstimatedAnonymity}");
            }
        }

        return 0;
    }

    public static string UnpackValue(Value protoValue)
    {
        var jsonValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(protoValue.ToString());
        return jsonValue.Values.ElementAt(0).ToString();
    }
}

// [END dlp_k_map]
