// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Events.SystemTextJson.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public class HelloGcsGenericTest : FunctionTestBase<HelloGcsGeneric.Function>
    {
        [Fact]
        public async Task CloudEventIsLogged()
        {
            var client = Server.CreateClient();

            var created = DateTimeOffset.UtcNow.AddMinutes(-5);
            var updated = created.AddMinutes(2);
            var data = new StorageObjectData
            {
                Name = "new-file.txt",
                Bucket = "my-bucket",
                Metageneration = 23,
                TimeCreated = new DateTimeOffset(2020, 7, 9, 13, 0, 5, TimeSpan.Zero),
                Updated = new DateTimeOffset(2020, 7, 9, 13, 23, 25, TimeSpan.Zero)
            };
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("uri", UriKind.Relative),
                // CloudEvent headers
                Headers =
                {
                    { "ce-type", StorageObjectData.DeletedCloudEventType },
                    { "ce-id", "1234" },
                    { "ce-source", "//storage.googleapis.com/" },
                    { "ce-specversion", "1.0" }
                },
                Content = new StringContent(JsonSerializer.Serialize(data)),
                Method = HttpMethod.Post
            };
            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var logs = Server.GetLogEntries(typeof(HelloGcsGeneric.Function));
            Assert.All(logs, entry => Assert.Equal(LogLevel.Information, entry.Level));

            var actualMessages = logs.Select(entry => entry.Message).ToList();
            var expectedMessages = new[]
            {
                $"Event: 1234",
                $"Event Type: {StorageObjectData.DeletedCloudEventType}",
                "Bucket: my-bucket",
                "File: new-file.txt",
                "Metageneration: 23",
                "Created: 2020-07-09T13:00:05",
                "Updated: 2020-07-09T13:23:25",
            };

            Assert.Equal(expectedMessages, actualMessages);
        }
    }
}
