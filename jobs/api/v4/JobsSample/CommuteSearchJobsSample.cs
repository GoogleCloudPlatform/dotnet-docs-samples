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
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using System;

namespace GoogleCloudSamples
{
    internal class CommuteSearchJobsSample
    {
        // [START job_search_commute_search]
        public static object CommuteSearchJobs(string projectId, string tenantId)
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

            CommuteMethod commuteMethod = CommuteMethod.Driving;
            long seconds = 3600L;
            Duration travelDuration = new Duration
            {
                Seconds = seconds
            };

            double latitude = 37.422408;
            double longitude = -122.084068;
            LatLng startCoordinates = new LatLng
            {
                Latitude = latitude,
                Longitude = longitude
            };

            CommuteFilter commuteFilter = new CommuteFilter
            {
                CommuteMethod = commuteMethod,
                TravelDuration = travelDuration,
                StartCoordinates = startCoordinates
            };

            JobQuery jobQuery = new JobQuery
            {
                CommuteFilter = commuteFilter
            };

            SearchJobsRequest request = new SearchJobsRequest
            {
                ParentAsTenantName = name,
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery
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
        // [END job_search_commute_search]
    }
}
