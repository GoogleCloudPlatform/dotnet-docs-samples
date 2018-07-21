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

using Google.Cloud.BigQuery.V2;
using Google.Cloud.Dlp.V2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

namespace GoogleCloudSamples
{
    class InspectSamples
    {
        // [START dlp_inspect_string]
        public static object InspectString(
            string projectId,
            string dataValue,
            string minLikelihood,
            int maxFindings,
            bool includeQuote,
            IEnumerable<InfoType> infoTypes,
            IEnumerable<CustomInfoType> customInfoTypes)
        {
            var inspectConfig = new InspectConfig
            {
                MinLikelihood = (Likelihood)System.Enum.Parse(typeof(Likelihood), minLikelihood),
                Limits = new InspectConfig.Types.FindingLimits
                {
                    MaxFindingsPerRequest = maxFindings
                },
                IncludeQuote = includeQuote,
                InfoTypes = { infoTypes },
                CustomInfoTypes = { customInfoTypes }
            };
            var request = new InspectContentRequest
            {
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
                Item = new ContentItem
                {
                    Value = dataValue
                },
                InspectConfig = inspectConfig
            };

            DlpServiceClient dlp = DlpServiceClient.Create();
            InspectContentResponse response = dlp.InspectContent(request);

            var findings = response.Result.Findings;
            if (findings.Count > 0)
            {
                Console.WriteLine("Findings:");
                foreach (var finding in findings)
                {
                    if (includeQuote)
                    {
                        Console.WriteLine($"  Quote: {finding.Quote}");
                    }
                    Console.WriteLine($"  InfoType: {finding.InfoType}");
                    Console.WriteLine($"  Likelihood: {finding.Likelihood}");
                }
            }
            else
            {
                Console.WriteLine("No findings.");
            }

            return 0;
        }
        // [END dlp_inspect_string]

        // [START dlp_inspect_file]
        static readonly Dictionary<string, ByteContentItem.Types.BytesType> s_fileTypes =
            new Dictionary<string, ByteContentItem.Types.BytesType>()
        {
            { ".bmp", ByteContentItem.Types.BytesType.ImageBmp },
            { ".jpg", ByteContentItem.Types.BytesType.ImageJpeg },
            { ".jpeg", ByteContentItem.Types.BytesType.ImageJpeg },
            { ".png", ByteContentItem.Types.BytesType.ImagePng },
            { ".svg", ByteContentItem.Types.BytesType.ImageSvg },
            { ".txt", ByteContentItem.Types.BytesType.TextUtf8 }
        };

        public static object InspectFile(
            string projectId,
            string file,
            string minLikelihood,
            int maxFindings,
            bool includeQuote,
            IEnumerable<InfoType> infoTypes,
            IEnumerable<CustomInfoType> customInfoTypes)
        {
            var fileStream = new FileStream(file, FileMode.Open);
            try
            {
                var inspectConfig = new InspectConfig
                {
                    MinLikelihood = (Likelihood)System.Enum.Parse(typeof(Likelihood), minLikelihood),
                    Limits = new FindingLimits
                    {
                        MaxFindingsPerRequest = maxFindings
                    },
                    IncludeQuote = includeQuote,
                    InfoTypes = { infoTypes },
                    CustomInfoTypes = { customInfoTypes }
                };
                DlpServiceClient dlp = DlpServiceClient.Create();
                InspectContentResponse response = dlp.InspectContent(new InspectContentRequest
                {
                    ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
                    Item = new ContentItem
                    {
                        ByteItem = new ByteContentItem
                        {
                            Data = ByteString.FromStream(fileStream),
                            Type = s_fileTypes.GetValueOrDefault(
                                    new FileInfo(file).Extension.ToLower(),
                                    ByteContentItem.Types.BytesType.Unspecified
                            )
                        }
                    },
                    InspectConfig = inspectConfig
                });

                var findings = response.Result.Findings;
                if (findings.Count > 0)
                {
                    Console.WriteLine("Findings:");
                    foreach (var finding in findings)
                    {
                        if (includeQuote)
                        {
                            Console.WriteLine($"  Quote: {finding.Quote}");
                        }
                        Console.WriteLine($"  InfoType: {finding.InfoType}");
                        Console.WriteLine($"  Likelihood: {finding.Likelihood}");
                    }
                }
                else
                {
                    Console.WriteLine("No findings.");
                }

                return 0;
            }
            finally
            {
                fileStream.Close();
            }
        }
        // [END dlp_inspect_file]

        // [START dlp_inspect_bigquery]
        public static object InspectBigQuery(
            string projectId,
            string minLikelihood,
            int maxFindings,
            bool includeQuote,
            IEnumerable<FieldId> identifyingFields,
            IEnumerable<InfoType> infoTypes,
            IEnumerable<CustomInfoType> customInfoTypes,
            string datasetId,
            string tableId)
        {
            var inspectJob = new InspectJobConfig
            {
                StorageConfig = new StorageConfig
                {
                    BigQueryOptions = new BigQueryOptions
                    {
                        TableReference = new Google.Cloud.Dlp.V2.BigQueryTable
                        {
                            ProjectId = projectId,
                            DatasetId = datasetId,
                            TableId = tableId,
                        },
                        IdentifyingFields =
                        {
                            identifyingFields
                        }
                    },

                    TimespanConfig = new StorageConfig.Types.TimespanConfig
                    {
                        StartTime = Timestamp.FromDateTime(System.DateTime.UtcNow.AddYears(-1)),
                        EndTime = Timestamp.FromDateTime(System.DateTime.UtcNow)
                    }
                },

                InspectConfig = new InspectConfig
                {
                    InfoTypes = { infoTypes },
                    CustomInfoTypes = { customInfoTypes },
                    Limits = new FindingLimits
                    {
                        MaxFindingsPerRequest = maxFindings
                    },
                    ExcludeInfoTypes = false,
                    IncludeQuote = includeQuote,
                    MinLikelihood = (Likelihood)System.Enum.Parse(typeof(Likelihood), minLikelihood)
                },
                Actions =
                {
                    new Google.Cloud.Dlp.V2.Action
                    {
                        // Save results in BigQuery Table
                        SaveFindings = new Google.Cloud.Dlp.V2.Action.Types.SaveFindings
                        {
                            OutputConfig = new OutputStorageConfig
                            {
                                Table = new Google.Cloud.Dlp.V2.BigQueryTable
                                {
                                    ProjectId = projectId,
                                    DatasetId = datasetId,
                                    TableId = tableId
                                }
                            }
                        },
                    }
                }
            };

            // Issue Create Dlp Job Request
            DlpServiceClient client = DlpServiceClient.Create();
            var request = new CreateDlpJobRequest
            {
                InspectJob = inspectJob,
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
            };

            // We need created job name
            var dlpJob = client.CreateDlpJob(request);
            string jobName = dlpJob.Name;

            // Make sure the job finishes before inspecting the results.
            // Alternatively, we can inspect results opportunistically, but 
            // for testing purposes, we want consistent outcome
            bool jobFinished = EnsureJobFinishes(projectId, jobName);
            if (jobFinished)
            {
                var bigQueryClient = BigQueryClient.Create(projectId);
                var table = bigQueryClient.GetTable(datasetId, tableId);

                // Return only first page of 10 rows
                Console.WriteLine("DLP v2 Results:");
                var firstPage = table.ListRows(new ListRowsOptions { StartIndex = 0, PageSize = 10 });
                foreach (var item in firstPage)
                {
                    Console.WriteLine($"\t {item[""]}");
                }
            }

            return 0;
        }
        // [END dlp_inspect_bigquery]

        // [START dlp_inspect_datastore]
        public static object InspectCloudDataStore(
            string projectId,
            string minLikelihood,
            int maxFindings,
            bool includeQuote,
            string kindName,
            string namespaceId,
            IEnumerable<InfoType> infoTypes,
            IEnumerable<CustomInfoType> customInfoTypes,
            string datasetId,
            string tableId)
        {
            var inspectJob = new InspectJobConfig
            {
                StorageConfig = new StorageConfig
                {
                    DatastoreOptions = new DatastoreOptions
                    {
                        Kind = new KindExpression { Name = kindName },
                        PartitionId = new PartitionId
                        {
                            NamespaceId = namespaceId,
                            ProjectId = projectId,
                        }
                    },
                    TimespanConfig = new StorageConfig.Types.TimespanConfig
                    {
                        StartTime = Timestamp.FromDateTime(System.DateTime.UtcNow.AddYears(-1)),
                        EndTime = Timestamp.FromDateTime(System.DateTime.UtcNow)
                    }
                },

                InspectConfig = new InspectConfig
                {
                    InfoTypes = { infoTypes },
                    CustomInfoTypes = { customInfoTypes },
                    Limits = new FindingLimits
                    {
                        MaxFindingsPerRequest = maxFindings
                    },
                    ExcludeInfoTypes = false,
                    IncludeQuote = includeQuote,
                    MinLikelihood = (Likelihood)System.Enum.Parse(typeof(Likelihood), minLikelihood)
                },
                Actions =
                {
                    new Google.Cloud.Dlp.V2.Action
                    {
                        // Save results in BigQuery Table
                        SaveFindings = new Google.Cloud.Dlp.V2.Action.Types.SaveFindings
                        {
                            OutputConfig = new OutputStorageConfig
                            {
                                Table = new Google.Cloud.Dlp.V2.BigQueryTable
                                {
                                    ProjectId = projectId,
                                    DatasetId = datasetId,
                                    TableId = tableId
                                }
                            }
                        },
                    }
                }
            };

            // Issue Create Dlp Job Request
            DlpServiceClient client = DlpServiceClient.Create();
            var request = new CreateDlpJobRequest
            {
                InspectJob = inspectJob,
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
            };

            // We need created job name
            var dlpJob = client.CreateDlpJob(request);
            var jobName = dlpJob.Name;

            // Make sure the job finishes before inspecting the results.
            // Alternatively, we can inspect results opportunistically, but
            // for testing purposes, we want consistent outcome
            bool jobFinished = EnsureJobFinishes(projectId, jobName);
            if (jobFinished)
            {
                var bigQueryClient = BigQueryClient.Create(projectId);
                var table = bigQueryClient.GetTable(datasetId, tableId);

                // Return only first page of 10 rows
                Console.WriteLine("DLP v2 Results:");
                var firstPage = table.ListRows(new ListRowsOptions { StartIndex = 0, PageSize = 10 });
                foreach (var item in firstPage)
                {
                    Console.WriteLine($"\t {item[""]}");
                }
            }

            return 0;
        }
        // [END dlp_inspect_datastore]


        // [START dlp_inspect_gcs]

        public static object InspectGCS(
            string projectId,
            string minLikelihood,
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
                    MinLikelihood = (Likelihood)System.Enum.Parse(typeof(Likelihood), minLikelihood)
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
            DlpServiceClient client = DlpServiceClient.Create();
            var request = new CreateDlpJobRequest
            {
                InspectJob = inspectJob,
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(projectId),
            };

            // We need created job name
            var dlpJob = client.CreateDlpJob(request);

            // Get a pub/sub subscription and listen for DLP results
            var fireEvent = new ManualResetEventSlim();

            var subscriptionName = new SubscriptionName(projectId, subscriptionId);
            var subscriberClient = SubscriberServiceApiClient.Create();
            var subscriber = SubscriberClient.Create(subscriptionName, new[] { subscriberClient });
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
            }
            else
            {
                Console.WriteLine("Error: The wait failed on timeout");
            }

            return 0;
        }

        // [END dlp_inspect_gcs]

        static bool EnsureJobFinishes(string projectId, string jobName)
        {
            DlpServiceClient client = DlpServiceClient.Create();
            var request = new GetDlpJobRequest
            {
                DlpJobName = new DlpJobName(projectId, jobName),
            };

            // Simple logic that gives the job 5*30 sec at most to complete - for testing purposes only
            int numOfAttempts = 5;
            do
            {
                var dlpJob = client.GetDlpJob(request);
                numOfAttempts--;
                if (dlpJob.State != DlpJob.Types.JobState.Running)
                {
                    return true;
                }

                Thread.Sleep(TimeSpan.FromSeconds(30));
            } while (numOfAttempts > 0);

            return false;
        }
    }
}
