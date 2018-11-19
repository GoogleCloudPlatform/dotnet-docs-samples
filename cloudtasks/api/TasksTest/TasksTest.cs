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
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace GoogleCloudSamples
{
    public class TasksTestFixture
    {
        public readonly string ProjectId;
        public readonly string QueueId;
        public readonly string WrappedKey;
        public readonly string KeyName;

        public readonly CommandLineRunner CommandLineRunner = new CommandLineRunner
        {
            VoidMain = Tasks.Main,
        };

        public TasksTestFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            QueueId = Environment.GetEnvironmentVariable("GCP_QUEUE");
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
        }
    }

    public partial class TasksTest : IClassFixture<TasksTestFixture>
    {
        private readonly TasksTestFixture _fixture;
        private string ProjectId { get { return _fixture.ProjectId; } }
        private string QueueId { get { return _fixture.QueueId; } }
        private readonly RetryRobot _retryRobot = new RetryRobot();

        readonly CommandLineRunner _tasks = new CommandLineRunner()
        {
            VoidMain = Tasks.Main,
            Command = "Tasks"
        };

        public TasksTest(TasksTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Dispose()
        {
            // TODO add cleanup logic here
        }

        [Fact]
        public void TestCreateTask()
        {
            ConsoleOutput output = _tasks.Run(
                "createTask",
                "--projectId", ProjectId,
                "--location", "us-central1",
                "--queue", QueueId,
                "--payload", "Test Payload"
            );
            Assert.Contains("Created Task", output.Stdout);
        }
    }
}
