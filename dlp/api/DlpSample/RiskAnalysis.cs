using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using static Google.Cloud.Dlp.V2.Action.Types;
using static Google.Cloud.Dlp.V2.PrivacyMetric.Types;

namespace GoogleCloudSamples
{
    abstract class RiskAnalysisOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string CallingProjectId { get; set; }

        [Value(1, HelpText = "The project ID the table is stored under.", Required = true)]
        public string TableProjectId { get; set; }

        [Value(2, HelpText = "The ID of the dataset to inspect. (e.g. 'my_dataset')", Required = true)]
        public string DatasetId { get; set; }

        [Value(3, HelpText = "The ID of the table to inspect. (e.g. 'my_table')", Required = true)]
        public string TableId { get; set; }

        [Value(4, HelpText = "The name of the Pub/Sub topic to notify once the job completes.", Default = 0)]
        public string TopicId { get; set; }

        [Value(5, HelpText = "The name of the Pub/Sub subscription to use when listening for job completion notifications.", Default = 0)]
        public string SubscriptionId { get; set; }
    }

    abstract class StatsOptions : RiskAnalysisOptions
    {
        [Value(6, HelpText = "The name of the column to compute risk metrics for. (e.g. 'age')", Default = 0)]
        public string ColumnName { get; set; }
    }

    abstract class QuasiIdOptions : RiskAnalysisOptions
    {
        [Value(6, HelpText = "A set of columns that form a composite key, delimited by commas. (e.g. 'name,city')", Required = true)]
        public string QuasiIdColumns { get; set; }
    }

    [Verb("numericalStats", HelpText = "Computes risk metrics of a column of numbers in a Google BigQuery table.")]
    abstract class NumericalStatsOptions : StatsOptions { }

    [Verb("categoricalStats", HelpText = "Computes risk metrics of a column of data in a Google BigQuery table.")]
    abstract class CategoricalStatsOptions : StatsOptions { }

    [Verb("kAnonymity", HelpText = "Computes the k-anonymity of a column set in a Google BigQuery table.")]
    abstract class KAnonymityOptions : QuasiIdOptions { }

    [Verb("lDiversity", HelpText = "Computes the k-anonymity of a column set in a Google BigQuery table.")]
    abstract class LDiversityOptions : QuasiIdOptions
    {
        [Value(7, HelpText = "The column to measure l-diversity relative to. (e.g. 'age')", Required = true)]
        public string SensitiveAttribute { get; set; }
    }

    [Verb("kMap", HelpText = "Computes the k-map risk estimation of a column set in a Google BigQuery table.")]
    abstract class KMapOptions : QuasiIdOptions
    {
        [Value(7, HelpText = "A list of the infoTypes for each quasi-id, delimited by commas.", Required = true)]
        public string InfoTypes { get; set; }

        [Value(8, HelpText = "The ISO 3166-1 region code that the data is representative of.", Default = "")]
        public string RegionCode { get; set; }
    }

    public class RiskAnalysis
    {
        static object NumericalStats(NumericalStatsOptions opts) {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric {
                    NumericalStatsConfig = new NumericalStatsConfig {
                        Field = new FieldId { Name = opts.ColumnName }
                    }
                },
                SourceTable = new BigQueryTable {
                    ProjectId = opts.TableProjectId,
                    DatasetId = opts.DatasetId,
                    TableId = opts.TableId
                }
            };
            config.Actions.Add(new Google.Cloud.Dlp.V2.Action
            {
                PubSub = new PublishToPubSub{
                    Topic = opts.TopicId
                }
            });

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest {
                Parent = $"projects/{opts.CallingProjectId}",
                RiskJob = config
            });

            // Listen to pub/sub for the job
            SubscriptionName subscriptionName = new SubscriptionName(opts.CallingProjectId,
    opts.SubscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            ManualResetEvent done = new ManualResetEvent(false);
            subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                if (message.Attributes["DlpJobName"] == submittedJob.Name)
                {
                    done.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                else
                {
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });

            done.WaitOne();
            subscriber.StopAsync(CancellationToken.None).Wait();

            // Process results
            var resultJob = dlp.GetDlpJob(new GetDlpJobRequest
            {
                DlpJobName = DlpJobName.Parse(submittedJob.Name)
            });

            var result = resultJob.RiskDetails.NumericalStatsResult;

            Console.WriteLine($"Value Range: [{result.MinValue}, {result.MaxValue}]");
            string lastValue = "";
            for (int quantile = 0; quantile < result.QuantileValues.Count; quantile++)
            {
                string value = result.QuantileValues[quantile].ToString(); // TODO print this nicer
                if (lastValue != value)
                {
                    Console.WriteLine($"Value at {quantile + 1}% quantile: ${value}");
                }
                lastValue = value;
            }

            return 0;
        }

        static object CategoricalStats(CategoricalStatsOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    CategoricalStatsConfig = new CategoricalStatsConfig()
                    {
                        Field = new FieldId { Name = opts.ColumnName }
                    }
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = opts.TableProjectId,
                    DatasetId = opts.DatasetId,
                    TableId = opts.TableId
                }
            };
            config.Actions.Add(new Google.Cloud.Dlp.V2.Action
            {
                PubSub = new PublishToPubSub
                {
                    Topic = opts.TopicId
                }
            });

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest
            {
                Parent = $"projects/{opts.CallingProjectId}",
                RiskJob = config
            });

            // Listen to pub/sub for the job
            SubscriptionName subscriptionName = new SubscriptionName(opts.CallingProjectId,
    opts.SubscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            ManualResetEvent done = new ManualResetEvent(false);
            subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                if (message.Attributes["DlpJobName"] == submittedJob.Name)
                {
                    done.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                else
                {
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });

            done.WaitOne();
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
                Console.WriteLine($"  {bucket.BucketSize} unique values total.");

                foreach (var bucketValue in bucket.BucketValues) {
                    Console.WriteLine($"  Value {bucketValue.Value.ToString()} occurs {bucketValue.Count} time(s).");
                }
            }

            return 0;
        }

        static object KAnonymity(KAnonymityOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            KAnonymityConfig KAnonymityConfig = new KAnonymityConfig();
            foreach (string QuasiId in opts.QuasiIdColumns.Split(',')) {
                KAnonymityConfig.QuasiIds.Add(new FieldId{ Name = QuasiId });
            }
            
            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    KAnonymityConfig = KAnonymityConfig
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = opts.TableProjectId,
                    DatasetId = opts.DatasetId,
                    TableId = opts.TableId
                }
            };
            config.Actions.Add(new Google.Cloud.Dlp.V2.Action
            {
                PubSub = new PublishToPubSub
                {
                    Topic = opts.TopicId
                }
            });

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest
            {
                Parent = $"projects/{opts.CallingProjectId}",
                RiskJob = config
            });

            // Listen to pub/sub for the job
            SubscriptionName subscriptionName = new SubscriptionName(opts.CallingProjectId,
    opts.SubscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            ManualResetEvent done = new ManualResetEvent(false);
            subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                if (message.Attributes["DlpJobName"] == submittedJob.Name)
                {
                    done.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                else
                {
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });

            done.WaitOne();
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
                Console.WriteLine($"  {bucket.BucketSize} unique values total.");

                foreach (var bucketValue in bucket.BucketValues)
                {
                    Console.WriteLine($"    Quasi-ID values: [{String.Join(',', bucketValue.QuasiIdsValues.Select(x => x.ToString()))}]");
                    Console.WriteLine($"    Class size: {bucketValue.EquivalenceClassSize}");
                }
            }

            return 0;
        }

        static object LDiversity(LDiversityOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            LDiversityConfig LDiversityConfig = new LDiversityConfig{
                SensitiveAttribute = new FieldId {  Name = opts.SensitiveAttribute }
            };
            foreach (string QuasiId in opts.QuasiIdColumns.Split(','))
            {
                LDiversityConfig.QuasiIds.Add(new FieldId { Name = QuasiId });
            }

            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    LDiversityConfig = LDiversityConfig
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = opts.TableProjectId,
                    DatasetId = opts.DatasetId,
                    TableId = opts.TableId
                }
            };
            config.Actions.Add(new Google.Cloud.Dlp.V2.Action
            {
                PubSub = new PublishToPubSub
                {
                    Topic = opts.TopicId
                }
            });

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest
            {
                Parent = $"projects/{opts.CallingProjectId}",
                RiskJob = config
            });

            // Listen to pub/sub for the job
            SubscriptionName subscriptionName = new SubscriptionName(opts.CallingProjectId,
    opts.SubscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            ManualResetEvent done = new ManualResetEvent(false);
            subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                if (message.Attributes["DlpJobName"] == submittedJob.Name)
                {
                    done.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                else
                {
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });

            done.WaitOne();
            subscriber.StopAsync(CancellationToken.None).Wait();

            // Process results
            var resultJob = dlp.GetDlpJob(new GetDlpJobRequest
            {
                DlpJobName = DlpJobName.Parse(submittedJob.Name)
            });

            var result = resultJob.RiskDetails.LDiversityResult;

            for (int bucketIdx = 0; bucketIdx < result.SensitiveValueFrequencyHistogramBuckets.Count; bucketIdx++)
            {
                var bucket = result.SensitiveValueFrequencyHistogramBuckets[bucketIdx];
                Console.WriteLine($"Bucket {bucketIdx}");
                Console.WriteLine($"  Bucket size range: [{bucket.SensitiveValueFrequencyLowerBound}, {bucket.SensitiveValueFrequencyUpperBound}].");
                Console.WriteLine($"  {bucket.BucketSize} unique values total.");

                foreach (var bucketValue in bucket.BucketValues)
                {
                    Console.WriteLine($"    Quasi-ID values: [{String.Join(',', bucketValue.QuasiIdsValues.Select(x => x.ToString()))}]");
                    Console.WriteLine($"    Class size: {bucketValue.EquivalenceClassSize}");

                    foreach (var topValue in bucketValue.TopSensitiveValues) {
                        Console.WriteLine($"    Sensitive value {topValue.Value.ToString()} occurs {topValue.Count} time(s).");
                    }
                }
            }

            return 0;
        }

        static object KMap(KMapOptions opts)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            // Construct + submit the job
            KAnonymityConfig KAnonymityConfig = new KAnonymityConfig();
            foreach (string QuasiId in opts.QuasiIdColumns.Split(','))
            {
                KAnonymityConfig.QuasiIds.Add(new FieldId { Name = QuasiId });
            }

            RiskAnalysisJobConfig config = new RiskAnalysisJobConfig
            {
                PrivacyMetric = new PrivacyMetric
                {
                    KAnonymityConfig = KAnonymityConfig
                },
                SourceTable = new BigQueryTable
                {
                    ProjectId = opts.TableProjectId,
                    DatasetId = opts.DatasetId,
                    TableId = opts.TableId
                }
            };
            config.Actions.Add(new Google.Cloud.Dlp.V2.Action
            {
                PubSub = new PublishToPubSub
                {
                    Topic = opts.TopicId
                }
            });

            var submittedJob = dlp.CreateDlpJob(new CreateDlpJobRequest
            {
                Parent = $"projects/{opts.CallingProjectId}",
                RiskJob = config
            });

            // Listen to pub/sub for the job
            SubscriptionName subscriptionName = new SubscriptionName(opts.CallingProjectId,
    opts.SubscriptionId);
            SubscriberClient subscriber = SubscriberClient.Create(
                subscriptionName, new[] { SubscriberServiceApiClient.Create() });
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            ManualResetEvent done = new ManualResetEvent(false);
            subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                if (message.Attributes["DlpJobName"] == submittedJob.Name)
                {
                    done.Set();
                    return Task.FromResult(SubscriberClient.Reply.Ack);
                }
                else
                {
                    return Task.FromResult(SubscriberClient.Reply.Nack);
                }
            });

            done.WaitOne();
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
                    Console.WriteLine($"    Values: [{String.Join(',', datapoint.QuasiIdsValues.Select(x => x.ToString()))}]");
                    Console.WriteLine($"    Estimated k-map anonymity: {datapoint.EstimatedAnonymity}");
                }
            }

            return 0;
        }

    }
}
