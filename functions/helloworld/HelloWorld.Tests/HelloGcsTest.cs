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

using Google.Events.Protobuf.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests
{
    public class HelloGcsTest : FunctionTestBase<HelloGcs.Function>
    {
        [Fact]
        public async Task CloudEventInput()
        {
            var data = new StorageObjectData { Name = "new-file.txt" };
            await ExecuteFunctionAsync(StorageObjectData.FinalizedCloudEventType, data);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal("File new-file.txt uploaded", logEntry.Message);
            Assert.Equal(LogLevel.Information, logEntry.Level);
        }

        [Fact]
        public async Task ObjectDeletedEvent()
        {
            var data = new StorageObjectData { Name = "new-file.txt" };
            await ExecuteFunctionAsync(StorageObjectData.DeletedCloudEventType, data);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal($"Unsupported event type: {StorageObjectData.DeletedCloudEventType}", logEntry.Message);
            Assert.Equal(LogLevel.Warning, logEntry.Level);
        }

        [Fact]
        public async Task NotCloudEvent()
        {
            var client = Server.CreateClient();
            var response = await client.GetAsync("uri");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Check that the cause of the failure is as expected. This is somewhat implementation-specific
            // (we're checking the logs for something that's not in this repo) but is easy to change if necessary,
            // and is an additional level of comfort.
            var errors = Server.GetLogEntries(typeof(Google.Cloud.Functions.Framework.CloudEventAdapter))
                .Where(entry => entry.Level == LogLevel.Error);
            var logEntry = Assert.Single(errors);
            Assert.Equal("Unable to convert request to CloudEvent.", logEntry.Message);
        }
    }
}
