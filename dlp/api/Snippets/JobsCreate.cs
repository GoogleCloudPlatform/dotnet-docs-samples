// Copyright (c) 2018 Google LLC.
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

// [START dlp_create_job]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;
using System.Linq;
using static Google.Cloud.Dlp.V2.StorageConfig.Types;

public class JobsCreate
{
    public static DlpJob CreateJob(string projectId, string gcsPath)
    {
        DlpServiceClient dlp = DlpServiceClient.Create();

        StorageConfig storageConfig = new StorageConfig
        {
            CloudStorageOptions = new CloudStorageOptions
            {
                FileSet = new CloudStorageOptions.Types.FileSet()
                {
                    Url = gcsPath
                }
            },
            TimespanConfig = new TimespanConfig
            {
                EnableAutoPopulationOfTimespanConfig = true
            }
        };

        InspectConfig inspectConfig = new InspectConfig
        {
            InfoTypes = { new[] { "EMAIL_ADDRESS", "CREDIT_CARD_NUMBER" }.Select(it => new InfoType() { Name = it }) },
            IncludeQuote = true,
            MinLikelihood = Likelihood.Unlikely,
            Limits = new InspectConfig.Types.FindingLimits() { MaxFindingsPerItem = 100 }
        };

        DlpJob response = dlp.CreateDlpJob(new CreateDlpJobRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            InspectJob = new InspectJobConfig
            {
                InspectConfig = inspectConfig,
                StorageConfig = storageConfig,
            }
        });

        Console.WriteLine($"Job: {response.Name} status: {response.State}");

        return response;
    }
}
// [END dlp_create_job]