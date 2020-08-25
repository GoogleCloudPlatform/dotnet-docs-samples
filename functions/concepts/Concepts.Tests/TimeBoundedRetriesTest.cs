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

using CloudNative.CloudEvents;
using Google.Cloud.Functions.Invoker.Testing;
using Google.Events;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Concepts.Tests
{
    public class TimeBoundedRetriesTest : FunctionTestBase<TimeBoundedRetries.Function>
    {
        // Note: In real code I'd use an IClock dependency to make this more reliably testable,
        // but I don't want to add that complexity into a sample.

        [Theory]
        [InlineData("Recent message", 1, "Processing PubSub message 'Recent message'")]
        [InlineData("Old message", 12, "Dropping PubSub message 'Old message'")]
        public async Task Processing(string textData, int ageInSeconds, string expectedLog)
        {
            var cloudEvent = new CloudEvent(MessagePublishedData.MessagePublishedCloudEventType,
                new Uri("//pubsub.googleapis.com"), "1234", DateTime.UtcNow.AddSeconds(-ageInSeconds));
            var data = new MessagePublishedData { Message = new PubsubMessage { TextData = textData } };
            CloudEventConverters.PopulateCloudEvent(cloudEvent, data);

            await ExecuteCloudEventRequestAsync(cloudEvent);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal(LogLevel.Information, logEntry.Level);
            Assert.Equal(expectedLog, logEntry.Message);
        }
    }
}
