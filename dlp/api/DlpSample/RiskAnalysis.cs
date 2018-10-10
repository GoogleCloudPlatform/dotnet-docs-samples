// Copyright 2018 Google Inc.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using static Google.Cloud.Dlp.V2.Action.Types;
using static Google.Cloud.Dlp.V2.PrivacyMetric.Types;
using static Google.Cloud.Dlp.V2.PrivacyMetric.Types.KMapEstimationConfig.Types;

namespace GoogleCloudSamples
{
    /// <summary>
    /// Examples of how to calculate various deidentification risk metrics for BigQuery tables 
    /// For more information, see https://cloud.google.com/dlp/docs/concepts-risk-analysis
    /// </summary>
    class RiskAnalysis
    {
        // [START dlp_numerical_stats]
        public static object NumericalStats(
            string callingProjectId,
            string tableProjectId,
            string datasetId,
            string tableId,
            string topicId,
            string subscriptionId,
            string columnName)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            var config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    NumericalStatsConfig = new NumericalStatsConfig
                    {
                        Field = new FieldId { Name = columnName }
                    }
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
                    ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(callingProjectId),
                    RiskJob = config
                });

            // Listen to pub/sub for the job
            var subscriptionName = new SubscriptionName(callingProjectId, subscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });

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
            var resultJob = dlp.GetDlpJob(
                new GetDlpJobRequest
                {
                    DlpJobName = DlpJobName.Parse(submittedJob.Name)
                });

            var result = resultJob.RiskDetails.NumericalStatsResult;

            // 'UnpackValue(x)' is a prettier version of 'x.toString()'
            Console.WriteLine($"Value Range: [{DlpSamplesUtils.UnpackValue(result.MinValue)}, {DlpSamplesUtils.UnpackValue(result.MaxValue)}]");
            string lastValue = string.Empty;
            for (int quantile = 0; quantile < result.QuantileValues.Count; quantile++)
            {
                string currentValue = DlpSamplesUtils.UnpackValue(result.QuantileValues[quantile]);
                if (lastValue != currentValue)
                {
                    Console.WriteLine($"Value at {quantile + 1}% quantile: {currentValue}");
                }
                lastValue = currentValue;
            }

            return 0;
        }
        // [END dlp_numerical_stats]

        // [START dlp_categorical_stats]
        public static object CategoricalStats(
            string callingProjectId,
            string tableProjectId,
            string datasetId,
            string tableId,
            string topicId,
            string subscriptionId,
            string columnName)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    CategoricalStatsConfig = new CategoricalStatsConfig()
                    {
                        Field = new FieldId { Name = columnName }
                    }
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = tableProjectId,
                    DatasetId = datasetId,
                    TableId = tableId
                },
                Actions = {
                    new Google.Cloud.Dlp.V2.Action
                    {
                        PubSub = new PublishToPubSub
                        {
                            Topic = $"projects/{callingProjectId}/topics/{topicId}"
                        }
                    }
                }
            };

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest
            {
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(callingProjectId),
                RiskJob = config
            });

            // Listen to pub/sub for the job
            var subscriptionName = new SubscriptionName(callingProjectId, subscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });

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

            var result = resultJob.RiskDetails.CategoricalStatsResult;

            for (int bucketIdx = 0; bucketIdx < result.ValueFrequencyHistogramBuckets.Count; bucketIdx++)
            {
                var bucket = result.ValueFrequencyHistogramBuckets[bucketIdx];
                Console.WriteLine($"Bucket {bucketIdx}");
                Console.WriteLine($"  Most common value occurs {bucket.ValueFrequencyUpperBound} time(s).");
                Console.WriteLine($"  Least common value occurs {bucket.ValueFrequencyLowerBound} time(s).");
                Console.WriteLine($"  {bucket.BucketSize} unique value(s) total.");

                foreach (var bucketValue in bucket.BucketValues)
                {
                    // 'UnpackValue(x)' is a prettier version of 'x.toString()'
                    Console.WriteLine($"  Value {DlpSamplesUtils.UnpackValue(bucketValue.Value)} occurs {bucketValue.Count} time(s).");
                }
            }

            return 0;
        }
        // [END dlp_categorical_stats]

        // [START dlp_k_anonymity]
        public static object KAnonymity(
            string callingProjectId,
            string tableProjectId,
            string datasetId,
            string tableId,
            string topicId,
            string subscriptionId,
            IEnumerable<FieldId> quasiIds)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            var KAnonymityConfig = new KAnonymityConfig
            {
                QuasiIds = { quasiIds }
            };

            var config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    KAnonymityConfig = KAnonymityConfig
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
                    ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(callingProjectId),
                    RiskJob = config
                });

            // Listen to pub/sub for the job
            var subscriptionName = new SubscriptionName(callingProjectId, subscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });

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

            var result = resultJob.RiskDetails.KAnonymityResult;

            for (int bucketIdx = 0; bucketIdx < result.EquivalenceClassHistogramBuckets.Count; bucketIdx++)
            {
                var bucket = result.EquivalenceClassHistogramBuckets[bucketIdx];
                Console.WriteLine($"Bucket {bucketIdx}");
                Console.WriteLine($"  Bucket size range: [{bucket.EquivalenceClassSizeLowerBound}, {bucket.EquivalenceClassSizeUpperBound}].");
                Console.WriteLine($"  {bucket.BucketSize} unique value(s) total.");

                foreach (var bucketValue in bucket.BucketValues)
                {
                    // 'UnpackValue(x)' is a prettier version of 'x.toString()'
                    Console.WriteLine($"    Quasi-ID values: [{String.Join(',', bucketValue.QuasiIdsValues.Select(x => DlpSamplesUtils.UnpackValue(x)))}]");
                    Console.WriteLine($"    Class size: {bucketValue.EquivalenceClassSize}");
                }
            }

            return 0;
        }
        // [END dlp_k_anonymity]

        // [START dlp_l_diversity]
        public static object LDiversity(
            string callingProjectId,
            string tableProjectId,
            string datasetId,
            string tableId,
            string topicId,
            string subscriptionId,
            IEnumerable<FieldId> quasiIds,
            string sensitiveAttribute)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            var ldiversityConfig = new LDiversityConfig
            {
                SensitiveAttribute = new FieldId { Name = sensitiveAttribute },
                QuasiIds = { quasiIds }
            };

            var config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    LDiversityConfig = ldiversityConfig
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = tableProjectId,
                    DatasetId = datasetId,
                    TableId = tableId
                },
                Actions = {
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
                    ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(callingProjectId),
                    RiskJob = config
                });

            // Listen to pub/sub for the job
            var subscriptionName = new SubscriptionName(callingProjectId, subscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });

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
            var resultJob = dlp.GetDlpJob(
                new GetDlpJobRequest
                {
                    DlpJobName = DlpJobName.Parse(submittedJob.Name)
                });

            var result = resultJob.RiskDetails.LDiversityResult;

            for (int bucketIdx = 0; bucketIdx < result.SensitiveValueFrequencyHistogramBuckets.Count; bucketIdx++)
            {
                var bucket = result.SensitiveValueFrequencyHistogramBuckets[bucketIdx];
                Console.WriteLine($"Bucket {bucketIdx}");
                Console.WriteLine($"  Bucket size range: [{bucket.SensitiveValueFrequencyLowerBound}, {bucket.SensitiveValueFrequencyUpperBound}].");
                Console.WriteLine($"  {bucket.BucketSize} unique value(s) total.");

                foreach (var bucketValue in bucket.BucketValues)
                {
                    // 'UnpackValue(x)' is a prettier version of 'x.toString()'
                    Console.WriteLine($"    Quasi-ID values: [{String.Join(',', bucketValue.QuasiIdsValues.Select(x => DlpSamplesUtils.UnpackValue(x)))}]");
                    Console.WriteLine($"    Class size: {bucketValue.EquivalenceClassSize}");

                    foreach (var topValue in bucketValue.TopSensitiveValues)
                    {
                        Console.WriteLine($"    Sensitive value {DlpSamplesUtils.UnpackValue(topValue.Value)} occurs {topValue.Count} time(s).");
                    }
                }
            }

            return 0;
        }
        // [END dlp_l_diversity]

        // [START dlp_k_map]
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
            DlpServiceClient dlp = DlpServiceClient.Create();

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
                    ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(callingProjectId),
                    RiskJob = config
                });

            // Listen to pub/sub for the job
            var subscriptionName = new SubscriptionName(
                callingProjectId,
                subscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });

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

            for (int histogramIdx = 0; histogramIdx < result.KMapEstimationHistogram.Count; histogramIdx++)
            {
                var histogramValue = result.KMapEstimationHistogram[histogramIdx];
                Console.WriteLine($"Bucket {histogramIdx}");
                Console.WriteLine($"  Anonymity range: [{histogramValue.MinAnonymity}, {histogramValue.MaxAnonymity}].");
                Console.WriteLine($"  Size: {histogramValue.BucketSize}");

                foreach (var datapoint in histogramValue.BucketValues)
                {
                    // 'UnpackValue(x)' is a prettier version of 'x.toString()'
                    Console.WriteLine($"    Values: [{String.Join(',', datapoint.QuasiIdsValues.Select(x => DlpSamplesUtils.UnpackValue(x)))}]");
                    Console.WriteLine($"    Estimated k-map anonymity: {datapoint.EstimatedAnonymity}");
                }
            }

            return 0;
        }
        // [END dlp_k_map]
    }
}
