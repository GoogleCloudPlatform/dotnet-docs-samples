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

using CommandLine;
using Google.Cloud.Dlp.V2;
using System;
using System.Collections.Generic;
using static Google.Cloud.Dlp.V2.CloudStorageOptions.Types;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;
using static Google.Cloud.Dlp.V2.JobTrigger.Types;
using static Google.Cloud.Dlp.V2.StorageConfig.Types;

namespace GoogleCloudSamples
{
    [Verb("listJobTriggers", HelpText = "List Data Loss Prevention API triggers.")]
    class ListJobTriggersOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("deleteJobTrigger", HelpText = "Delete a Data Loss Prevention API job trigger.")]
    class DeleteJobTriggerOptions
    {
        [Value(0, HelpText = "The full name of the trigger to be deleted.", Required = true)]
        public string TriggerName { get; set; }
    }

    /// <summary>
    /// Examples of how to create, list, and delete DLP job triggers
    /// For more information, see https://cloud.google.com/dlp/docs/concepts-job-triggers
    /// </summary>
    class JobTriggers
    {
        // [START dlp_create_trigger]
        public static object CreateJobTrigger(
            string projectId,
            string bucketName,
            string minLikelihood,
            int maxFindings,
            bool autoPopulateTimespan,
            int scanPeriod,
            IEnumerable<InfoType> infoTypes,
            string triggerId,
            string displayName,
            string description)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            var jobConfig = new InspectJobConfig
            {
                InspectConfig = new InspectConfig
                {
                    MinLikelihood = (Likelihood)Enum.Parse(
                        typeof(Likelihood),
                        minLikelihood
                    ),
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

            JobTrigger response = dlp.CreateJobTrigger(
                new CreateJobTriggerRequest
                {
                    ParentAsProjectName = new ProjectName(projectId),
                    JobTrigger = jobTrigger,
                    TriggerId = triggerId
                });

            Console.WriteLine($"Successfully created trigger {response.Name}");
            return 0;
        }
        // [END dlp_create_trigger]

        // [START dlp_list_triggers]
        public static object ListJobTriggers(string projectId)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            var response = dlp.ListJobTriggers(
                new ListJobTriggersRequest
                {
                    ParentAsProjectName = new ProjectName(projectId)
                });

            foreach (var trigger in response)
            {
                Console.WriteLine($"Name: {trigger.Name}");
                Console.WriteLine($"  Created: {trigger.CreateTime.ToString()}");
                Console.WriteLine($"  Updated: {trigger.UpdateTime.ToString()}");
                Console.WriteLine($"  Display Name: {trigger.DisplayName}");
                Console.WriteLine($"  Description: {trigger.Description}");
                Console.WriteLine($"  Status: {trigger.Status}");
                Console.WriteLine($"  Error count: {trigger.Errors.Count}");
            }

            return 0;
        }
        // [END dlp_list_triggers]

        // [START dlp_delete_trigger]
        public static object DeleteJobTrigger(string triggerName)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            dlp.DeleteJobTrigger(
                new DeleteJobTriggerRequest
                {
                    Name = triggerName
                });

            Console.WriteLine($"Successfully deleted trigger {triggerName}.");
            return 0;
        }
        // [END dlp_delete_trigger]
    }
}
