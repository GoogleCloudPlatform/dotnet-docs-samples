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

using Google.Apis.Auth.OAuth2;
using System;
using Xunit;

public class TasksTest
{
    public readonly string ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public readonly string QueueId = Environment.GetEnvironmentVariable("GCP_QUEUE") ?? "my-queue";
    public readonly string LocationId = Environment.GetEnvironmentVariable("LOCATION_ID") ?? "us-central1";

    [Fact]
    public void TestCreateTask()
    {
        var snippet = new CreateAppEngineTask();
        Assert.Contains(
            $"projects/{ProjectId}/locations/{LocationId}/queues/{QueueId}/tasks/",
            snippet.CreateTask(ProjectId, LocationId, QueueId, "Test"));
    }

    [Fact]
    public void TestCreateHttpTask()
    {
        var Url = $"https://example.com/taskhandler";
        var snippet = new CreateHttpTask();
        Assert.Contains(
            $"projects/{ProjectId}/locations/{LocationId}/queues/{QueueId}/tasks/",
            snippet.CreateTask(ProjectId, LocationId, QueueId, Url, "Test"));
    }
}
