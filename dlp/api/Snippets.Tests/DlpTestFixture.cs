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

using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dlp.V2;
using System;
using System.IO;

/* Initialize environment variables for DLP tests */
public class DlpTestFixture
{
    public readonly string ProjectId;
    public readonly string WrappedKey;
    public readonly string KeyName;
    public readonly string ResourcePath = Path.GetFullPath("../../../resources/");

    public DlpTestFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Authorize the client using Application Default Credentials.
        // See: https://developers.google.com/identity/protocols/application-default-credentials
        _ = GoogleCredential.GetApplicationDefaultAsync().Result;

        // Fetch the test key from an environment variable
        KeyName = Environment.GetEnvironmentVariable("DLP_DEID_KEY_NAME");
        WrappedKey = Environment.GetEnvironmentVariable("DLP_DEID_WRAPPED_KEY");
    }

    public void Dispose()
    {
        // Delete any jobs created by the test.
        DlpServiceClient dlp = DlpServiceClient.Create();
        Google.Api.Gax.PagedEnumerable<ListDlpJobsResponse, DlpJob> result = dlp.ListDlpJobs(new ListDlpJobsRequest
        {
            ParentAsProjectName = new ProjectName(ProjectId),
            Type = DlpJobType.RiskAnalysisJob
        });
        foreach (DlpJob job in result)
        {
            dlp.DeleteDlpJob(new DeleteDlpJobRequest()
            {
                Name = job.Name
            });
        }
    }
}
