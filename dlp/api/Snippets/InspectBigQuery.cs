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

// [START dlp_inspect_bigquery]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Dlp.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class InspectBigQuery
{
    public static object Inspect(
        string projectId,
        Likelihood minLikelihood,
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
                MinLikelihood = minLikelihood
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
        var client = DlpServiceClient.Create();
        var request = new CreateDlpJobRequest
        {
            InspectJob = inspectJob,
            Parent = new LocationName(projectId, "global").ToString(),
        };

        // We need created job name
        var dlpJob = client.CreateDlpJob(request);
        var jobName = dlpJob.Name;

        // Make sure the job finishes before inspecting the results.
        // Alternatively, we can inspect results opportunistically, but
        // for testing purposes, we want consistent outcome
        var finishedJob = EnsureJobFinishes(projectId, jobName);
        var bigQueryClient = BigQueryClient.Create(projectId);
        var table = bigQueryClient.GetTable(datasetId, tableId);

        // Return only first page of 10 rows
        Console.WriteLine("DLP v2 Results:");
        var firstPage = table.ListRows(new ListRowsOptions { StartIndex = 0, PageSize = 10 });
        foreach (var item in firstPage)
        {
            Console.WriteLine($"\t {item[""]}");
        }

        return finishedJob;
    }

    private static DlpJob EnsureJobFinishes(string projectId, string jobName)
    {
        var client = DlpServiceClient.Create();
        var request = new GetDlpJobRequest
        {
            DlpJobName = new DlpJobName(projectId, jobName),
        };

        // Simple logic that gives the job 5*30 sec at most to complete - for testing purposes only
        var numOfAttempts = 5;
        do
        {
            var dlpJob = client.GetDlpJob(request);
            numOfAttempts--;
            if (dlpJob.State != DlpJob.Types.JobState.Running)
            {
                return dlpJob;
            }

            Thread.Sleep(TimeSpan.FromSeconds(30));
        } while (numOfAttempts > 0);

        throw new InvalidOperationException("Job did not complete in time");
    }
}

// [END dlp_inspect_bigquery]
