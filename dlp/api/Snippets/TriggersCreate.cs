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

// [START dlp_create_trigger]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using static Google.Cloud.Dlp.V2.CloudStorageOptions.Types;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;
using static Google.Cloud.Dlp.V2.JobTrigger.Types;
using static Google.Cloud.Dlp.V2.StorageConfig.Types;

public class TriggersCreate
{
    public static JobTrigger Create(
        string projectId,
        string bucketName,
        Likelihood minLikelihood,
        int maxFindings,
        bool autoPopulateTimespan,
        int scanPeriod,
        IEnumerable<InfoType> infoTypes,
        string triggerId,
        string displayName,
        string description)
    {
        var dlp = DlpServiceClient.Create();

        var jobConfig = new InspectJobConfig
        {
            InspectConfig = new InspectConfig
            {
                MinLikelihood = minLikelihood,
                Limits = new FindingLimits
                {
                    MaxFindingsPerRequest = maxFindings
                },
                InfoTypes = { infoTypes }
            },
            StorageConfig = new StorageConfig
            {
                CloudStorageOptions = new CloudStorageOptions
                {
                    FileSet = new FileSet
                    {
                        Url = $"gs://{bucketName}/*"
                    }
                },
                TimespanConfig = new TimespanConfig
                {
                    EnableAutoPopulationOfTimespanConfig = autoPopulateTimespan
                }
            }
        };

        var jobTrigger = new JobTrigger
        {
            Triggers =
            {
                new Trigger
                {
                    Schedule = new Schedule
                    {
                        RecurrencePeriodDuration = new Google.Protobuf.WellKnownTypes.Duration
                        {
                            Seconds = scanPeriod * 60 * 60 * 24
                        }
                    }
                }
            },
            InspectJob = jobConfig,
            Status = Status.Healthy,
            DisplayName = displayName,
            Description = description
        };

        var response = dlp.CreateJobTrigger(
            new CreateJobTriggerRequest
            {
                Parent = new LocationName(projectId, "global").ToString(),
                JobTrigger = jobTrigger,
                TriggerId = triggerId
            });

        Console.WriteLine($"Successfully created trigger {response.Name}");
        return response;
    }
}

// [END dlp_create_trigger]
