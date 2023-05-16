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

// [START dlp_k_anonymity_with_entity_id]

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Newtonsoft.Json;

public class CalculateKAnonymityOnDataset
{
    public static DlpJob CalculateKAnonymitty(
        string projectId,
        string datasetId,
        string sourceTableId,
        string outputTableId)
    {
        // Construct the dlp client.
        var dlp = DlpServiceClient.Create();

        // Construct the k-anonymity config by setting the EntityId as user_id column
        // and two quasi-identifiers columns.
        var kAnonymity = new PrivacyMetric.Types.KAnonymityConfig
        {
            EntityId = new EntityId
            {
                Field = new FieldId { Name = "Name" }
            },
            QuasiIds =
            {
                new FieldId { Name = "Age" },
                new FieldId { Name = "Mystery" }
            }
        };

        // Construct risk analysis job config by providing the source table, privacy metric
        // and action to save the findings to a BigQuery table.
        var riskJob = new RiskAnalysisJobConfig
        {
            SourceTable = new BigQueryTable
            {
                ProjectId = projectId,
                DatasetId = datasetId,
                TableId = sourceTableId,
            },
            PrivacyMetric = new PrivacyMetric
            {
                KAnonymityConfig = kAnonymity,
            },
            Actions =
            {
                new Google.Cloud.Dlp.V2.Action
                {
                    SaveFindings = new Google.Cloud.Dlp.V2.Action.Types.SaveFindings
                    {
                        OutputConfig = new OutputStorageConfig
                        {
                            Table = new BigQueryTable
                            {
                                ProjectId = projectId,
                                DatasetId = datasetId,
                                TableId = outputTableId
                            }
                        }
                    }
                }
            }
        };

        // Construct the request by providing RiskJob object created above.
        var request = new CreateDlpJobRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            RiskJob = riskJob
        };

        // Send the job request.
        DlpJob response = dlp.CreateDlpJob(request);

        Console.WriteLine($"Job created successfully. Job name: ${response.Name}");

        return response;
    }
}

// [END dlp_k_anonymity_with_entity_id]
