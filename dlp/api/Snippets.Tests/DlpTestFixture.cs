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

using System;
using System.IO;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

namespace GoogleCloudSamples
{
    public class DlpTestFixture
    {
        public string ProjectId => Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public string WrappedKey => Environment.GetEnvironmentVariable("DLP_DEID_WRAPPED_KEY");
        public string KeyName => Environment.GetEnvironmentVariable("DLP_DEID_KEY_NAME");
        public string GcsFileUri => Environment.GetEnvironmentVariable("DLP_GCS_FILE_URI");
        public string TopicId => Environment.GetEnvironmentVariable("DLP_TOPIC_ID");
        public string SubscriptionId => Environment.GetEnvironmentVariable("DLP_SUBSCRIPTION_ID");
        public string ResourcePath => Path.GetFullPath("../../../resources/");

        public CreateDlpJobRequest GetTestRiskAnalysisJobRequest()
        {
            return new CreateDlpJobRequest()
            {
                Parent = new LocationName(ProjectId, "global").ToString(),
                RiskJob = new RiskAnalysisJobConfig()
                {
                    PrivacyMetric = new PrivacyMetric()
                    {
                        CategoricalStatsConfig = new PrivacyMetric.Types.CategoricalStatsConfig()
                        {
                            Field = new FieldId()
                            {
                                Name = "zip_code"
                            }
                        }
                    },
                    SourceTable = new BigQueryTable()
                    {
                        ProjectId = "bigquery-public-data",
                        DatasetId = "san_francisco",
                        TableId = "bikeshare_trips"
                    }
                }
            };
        }

        public DlpTestFixture() { }

        public void Dispose()
        {
            // Delete any jobs created by the test.
            var dlp = DlpServiceClient.Create();
            var result = dlp.ListDlpJobs(new ListDlpJobsRequest
            {
                Parent = new LocationName(ProjectId, "global").ToString(),
                Type = DlpJobType.RiskAnalysisJob
            });
            foreach (var job in result)
            {
                dlp.DeleteDlpJob(new DeleteDlpJobRequest()
                {
                    Name = job.Name
                });
            }
        }
    }
}
