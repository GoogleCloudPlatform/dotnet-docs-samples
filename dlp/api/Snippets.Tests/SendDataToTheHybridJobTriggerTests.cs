// Copyright 2023 Google Inc.
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
// limitations under the License

using Google.Cloud.Dlp.V2;
using Xunit;
using System.IO;
using Google.Api.Gax.ResourceNames;

namespace GoogleCloudSamples
{
    public class SendDataToTheHybridJobTriggerTests : IClassFixture<DlpTestFixture>
    {
        private DlpTestFixture _fixture;
        public SendDataToTheHybridJobTriggerTests(DlpTestFixture fixture) => _fixture = fixture;

        [Fact]
        public void TestHybridJob()
        {
            var dlp = DlpServiceClient.Create();

            // Create the Job Trigger.
            var request = new CreateJobTriggerRequest
            {
                ParentAsLocationName = new LocationName(_fixture.ProjectId, "global"),
                JobTrigger = new JobTrigger
                {
                    Triggers =
                    {
                        new JobTrigger.Types.Trigger
                        {
                            Manual = new Manual()
                        }
                    },
                    InspectJob = new InspectJobConfig
                    {
                        Actions =
                        {
                            new Action
                            {
                                JobNotificationEmails = new Action.Types.JobNotificationEmails(),
                                PublishToStackdriver = new Action.Types.PublishToStackdriver(),
                            }
                        },
                        InspectConfig = new InspectConfig
                        {
                            InfoTypes =
                            {
                                new InfoType[]
                                {
                                    new InfoType { Name = "EMAIL_ADDRESS" }
                                }
                            },
                            MinLikelihood = Likelihood.Possible,
                            IncludeQuote = true,
                        },
                        StorageConfig = new StorageConfig
                        {
                            HybridOptions = new HybridOptions
                            {
                                Description = "Hybrid job trigger for data from the comments field of a table that contains customer appointment bookings",
                            }
                        }
                    }
                }
            };
            var hybridJobTrigger = dlp.CreateJobTrigger(request);

            var text = "My email is test@example.org and name is Ariel.";

            DlpJob result = SendDataToTheHybridJobTrigger.SendToHybridJobTrigger(_fixture.ProjectId, hybridJobTrigger.JobTriggerName.JobTriggerId, text);

            // Delete the activated job.
            JobsDelete.DeleteJob(result.Name);

            // Delete the created job trigger.
            dlp.DeleteJobTrigger(new DeleteJobTriggerRequest
            {
                Name = hybridJobTrigger.Name
            });
        }
    }
}
