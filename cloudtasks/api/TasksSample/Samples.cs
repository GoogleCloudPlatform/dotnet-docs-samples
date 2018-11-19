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

using Google.Cloud.Tasks.V2Beta2;
using System;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Google.Protobuf;

namespace GoogleCloudSamples
{
    public partial class Samples
    {
        // [START cloud_tasks_appengine_create_task]
        public static object CreateTask(
            string projectId,
            string location,
            string queue,
            string payload,
            int inSeconds)
        {
            CloudTasksClient client = CloudTasksClient.Create();

            QueueName parent = new QueueName(projectId, location, queue);

            var UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var response = client.CreateTask(new CreateTaskRequest
            {
                Parent = parent.ToString(),
                Task = new Task
                {
                    AppEngineHttpRequest = new AppEngineHttpRequest
                    {
                        HttpMethod = HttpMethod.Post,
                        RelativeUrl = "/log_payload",
                        Payload = ByteString.CopyFromUtf8(payload)
                    },
                    ScheduleTime = new Timestamp {
                        Seconds = (long) (DateTime.Now.AddSeconds(inSeconds) - UnixEpoch).TotalSeconds,
                        Nanos = 0
                    }
                }
            });

            Console.WriteLine($"Created Task {response.Name}");

            return 0;
        }
        // [END cloud_tasks_appengine_create_task]
    }
}
