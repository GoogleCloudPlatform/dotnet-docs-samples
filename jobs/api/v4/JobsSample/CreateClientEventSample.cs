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

using static Google.Cloud.Talent.V4Beta1.JobEvent.Types;

using Google.Cloud.Talent.V4Beta1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace GoogleCloudSamples
{
    internal class CreateClientEventSample
    {
        // [START job_search_create_client_event]
        public static object CreateClientEvent(string projectId, string tenantId, string requestId, string eventId, IEnumerable<string> jobIds)
        {
            EventServiceClient eventServiceClient = EventServiceClient.Create();

            TenantName tenantName = TenantName.FromProjectTenant(projectId, tenantId);

            long seconds = 1L;
            Timestamp createTime = new Timestamp
            {
                Seconds = seconds
            };

            // The type of event attributed to the behavior of the end user.
            JobEventType type = JobEventType.View;

            // List of job names associated with this event.
            List<string> jobs = new List<string>();
            foreach (var jobId in jobIds)
            {
                //build full path of job IDs
                JobName name = JobName.FromProjectTenantJob(projectId, tenantId, jobId);
                jobs.Add(name.ToString());
            }

            JobEvent jobEvent = new JobEvent
            {
                Type = type
            };
            jobEvent.Jobs.Add(jobs);

            ClientEvent clientEvent = new ClientEvent
            {
                RequestId = requestId,
                EventId = eventId,
                CreateTime = createTime,
                JobEvent = jobEvent
            };

            CreateClientEventRequest request = new CreateClientEventRequest
            {
                ParentAsTenantName = tenantName,
                ClientEvent = clientEvent
            };

            ClientEvent response = eventServiceClient.CreateClientEvent(request);
            Console.WriteLine($"Created client event.");
            Console.WriteLine(response);

            return 0;
        }
        // [END job_search_create_client_event]
    }
}
