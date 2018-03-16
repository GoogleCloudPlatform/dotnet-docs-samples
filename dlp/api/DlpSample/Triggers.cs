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
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;
using static Google.Cloud.Dlp.V2.JobTrigger.Types;
using static Google.Cloud.Dlp.V2.CloudStorageOptions.Types;

namespace GoogleCloudSamples
{
    [Verb("createJobTrigger", HelpText = "Create a Data Loss Prevention API job trigger.")]
    class CreateJobTriggerOptions
    {
        [Value(0, HelpText = "The project ID to run the API call under.", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "The name of the bucket to scan.", Required = true)]
        public string BucketName { get; set; }

        [Value(2, HelpText = "The minimum likelihood required before returning a match.", Required = true)]
        public string MinLikelihood { get; set; }

        [Value(3, HelpText = "The maximum number of findings to report per request. (0 = server maximum)", Default = 0)]
        public int MaxFindings { get; set; }

        [Value(4, HelpText = "How often to wait between scans, in days. (minimum = 1 day)", Required = true)]
        public int ScanPeriod { get; set; }

        [Value(5, HelpText = "The name of the trigger to be created.", Default = "")]
        public string TriggerId { get; set; }

        [Value(6, HelpText = "A display name for the trigger to be created.", Default = "")]
        public string DisplayName { get; set; }

        [Value(7, HelpText = "A description for the trigger to be created.", Default = "")]
        public string Description { get; set; }
    }

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
    /// This class contains examples of how to create, list, and delete DLP job triggers
    /// For more information, see https://cloud.google.com/dlp/docs/concepts-job-triggers
    /// </summary>
    public class JobTriggers
    {
        static object CreateJobTrigger(
            string ProjectId,
            string BucketName,
            string MinLikelihood,
            int MaxFindings,
            int ScanPeriod,
            string TriggerId,
            string DisplayName,
            string Description)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            InspectJobConfig jobConfig = new InspectJobConfig {
                InspectConfig = new InspectConfig
                {
                    MinLikelihood = (Likelihood)Enum.Parse(
                        typeof(Likelihood),
                        MinLikelihood
                    ),
                    Limits = new FindingLimits
                    {
                        MaxFindingsPerRequest = MaxFindings
                    },
                    InfoTypes = {
                        new InfoType { Name = "PERSON_NAME" },
                        new InfoType { Name = "US_STATE" }
                    }
                },
                StorageConfig = new StorageConfig
                {
                    CloudStorageOptions = new CloudStorageOptions
                    {
                        FileSet = new FileSet
                        {
                            Url = $"gs://{BucketName}/*"
                        }
                    }
                }
            };

            JobTrigger jobTrigger = new JobTrigger {
                Triggers = {
                    new Trigger {
                        Schedule = new Schedule
                        {
                            RecurrencePeriodDuration = new Google.Protobuf.WellKnownTypes.Duration {
                                Seconds = ScanPeriod * 60 * 60 * 24
                            }
                        }
                    }
                },
                InspectJob = jobConfig,
                Status = Status.Healthy,
                DisplayName = DisplayName,
                Description = Description
            };

            JobTrigger response = dlp.CreateJobTrigger(new CreateJobTriggerRequest
            {
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(ProjectId),
                JobTrigger = jobTrigger,
                TriggerId = TriggerId
            });

            Console.WriteLine($"Successfully created trigger {response.Name}");
            return 0;
        }

        static object ListJobTriggers(string ProjectId) {
            DlpServiceClient dlp = DlpServiceClient.Create();

            var response = dlp.ListJobTriggers(new ListJobTriggersRequest
            {
                ParentAsProjectName = new Google.Cloud.Dlp.V2.ProjectName(ProjectId)
            });

            foreach (var trigger in response) {
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

        static object DeleteJobTrigger(string TriggerName)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();

            dlp.DeleteJobTrigger(new DeleteJobTriggerRequest
            {
                Name = TriggerName
            });

            Console.WriteLine($"Successfully deleted trigger {TriggerName}.");
            return 0;
        }
    }
}