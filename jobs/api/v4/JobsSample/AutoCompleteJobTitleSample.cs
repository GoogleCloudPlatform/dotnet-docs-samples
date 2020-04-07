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

using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class AutoCompleteJobTitleSample
    {
        // [START job_search_autocomplete_job_title]
        public static object CompleteQuery(string projectId, string tenantId, string query)
        {
            CompletionClient completionClient = CompletionClient.Create();
            TenantName tenant = TenantName.FromProjectTenant(projectId, tenantId);
            CompleteQueryRequest request = new CompleteQueryRequest
            {
                ParentAsTenantName = tenant,
                Query = query, // partial text for job title
                PageSize = 5, // limit for number of results
                LanguageCodes = { "en-US" } // language code
            };
            var response = completionClient.CompleteQuery(request);
            foreach (var result in response.CompletionResults)
            {
                Console.WriteLine($"Suggested title: {result.Suggestion}");
                // Suggestion type is JOB_TITLE or COMPANY_TITLE
                Console.WriteLine($"Suggestion type: {result.Type}");
            }

            return 0;
        }
        // [END job_search_autocomplete_job_title]
    }
}
