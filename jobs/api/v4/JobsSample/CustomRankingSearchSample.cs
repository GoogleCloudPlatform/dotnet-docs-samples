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

using static Google.Cloud.Talent.V4Beta1.SearchJobsRequest.Types;
using static Google.Cloud.Talent.V4Beta1.SearchJobsRequest.Types.CustomRankingInfo.Types;

using Google.Cloud.Talent.V4Beta1;
using System;

namespace GoogleCloudSamples
{
    internal class CustomRankingSearchSample
    {
        // [START job_search_custom_ranking_search]
        public static object CustomRankingSearch(string projectId, string tenantId)
        {
            JobServiceClient jobServiceClient = JobServiceClient.Create();
            TenantName name = TenantName.FromProjectTenant(projectId, tenantId);

            string domain = "www.example.com";
            string sessionId = "Hashed session identifier";
            string userId = "Hashed user identifier";
            RequestMetadata requestMetadata = new RequestMetadata
            {
                Domain = domain,
                SessionId = sessionId,
                UserId = userId
            };

            CustomRankingInfo customRankingInfo = new CustomRankingInfo
            {
                ImportanceLevel = ImportanceLevel.Extreme,
                // Custom ranking supports math operators, and Field name can be CPC or Freshness
                // https://cloud.google.com/talent-solution/job-search/docs/custom-ranking#how_to_use
                RankingExpression = "(someFieldLong + 25) * 0.25"
            };
            string orderBy = "custom_ranking desc";

            SearchJobsRequest request = new SearchJobsRequest
            {
                ParentAsTenantName = name,
                CustomRankingInfo = customRankingInfo,
                RequestMetadata = requestMetadata,
                OrderBy = orderBy
            };

            var response = jobServiceClient.SearchJobs(request);
            foreach (var result in response)
            {
                Console.WriteLine($"Job summary: {result.JobSummary}");
                Console.WriteLine($"Job title snippet: {result.JobTitleSnippet}");
                Job job = result.Job;
                Console.WriteLine($"Job name: {job.Name}");
                Console.WriteLine($"Job title: {job.Title}");
            }

            return 0;
        }
        // [END job_search_custom_ranking_search]
    }
}
