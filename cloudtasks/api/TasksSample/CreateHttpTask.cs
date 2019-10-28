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

// [START cloud_tasks_create_http_task]

using Google.Cloud.Tasks.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;

class CreateHttpTask
{
    public string CreateTask(
        string projectId = "YOUR-PROJECT-ID",
        string location = "us-central1",
        string queue = "my-queue",
        string url = "http://example.com/taskhandler",
        string payload = "Hello World!",
        int inSeconds = 0)
    {
        CloudTasksClient client = CloudTasksClient.Create();
        QueueName parent = new QueueName(projectId, location, queue);

        var response = client.CreateTask(new CreateTaskRequest
        {
            Parent = parent.ToString(),
            Task = new Task
            {
                HttpRequest = new HttpRequest
                {
                    HttpMethod = HttpMethod.Post,
                    Url = url,
                    Body = ByteString.CopyFromUtf8(payload)
                },
                ScheduleTime = Timestamp.FromDateTime(
                    DateTime.UtcNow.AddSeconds(inSeconds))
            }
        });

        Console.WriteLine($"Created Task {response.Name}");
        return response.Name;
    }
}
// [END cloud_tasks_create_http_task]
