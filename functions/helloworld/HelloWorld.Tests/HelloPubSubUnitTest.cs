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

// [START functions_cloudevent_pubsub_unit_test]
// [START functions_pubsub_unit_test]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Testing;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests;

public class HelloPubSubUnitTest
{
    [Fact]
    public async Task MessageWithTextData()
    {
        var data = new MessagePublishedData { Message = new PubsubMessage { TextData = "PubSub user" } };
        var cloudEvent = new CloudEvent
        {
            Type = MessagePublishedData.MessagePublishedCloudEventType,
            Source = new Uri("//pubsub.googleapis.com", UriKind.RelativeOrAbsolute),
            Id = Guid.NewGuid().ToString(),
            Time = DateTimeOffset.UtcNow,
            Data = data
        };

        var logger = new MemoryLogger<HelloPubSub.Function>();
        var function = new HelloPubSub.Function(logger);
        await function.HandleAsync(cloudEvent, data, CancellationToken.None);

        var logEntry = Assert.Single(logger.ListLogEntries());
        Assert.Equal("Hello PubSub user", logEntry.Message);
        Assert.Equal(LogLevel.Information, logEntry.Level);
    }

    [Fact]
    public async Task MessageWithoutTextData()
    {
        var data = new MessagePublishedData
        {
            Message = new PubsubMessage { Attributes = { { "key", "value" } } }
        };
        var cloudEvent = new CloudEvent
        {
            Type = MessagePublishedData.MessagePublishedCloudEventType,
            Source = new Uri("//pubsub.googleapis.com", UriKind.RelativeOrAbsolute),
            Id = Guid.NewGuid().ToString(),
            Time = DateTimeOffset.UtcNow
        };

        var logger = new MemoryLogger<HelloPubSub.Function>();
        var function = new HelloPubSub.Function(logger);
        await function.HandleAsync(cloudEvent, data, CancellationToken.None);

        var logEntry = Assert.Single(logger.ListLogEntries());
        Assert.Equal("Hello world", logEntry.Message);
        Assert.Equal(LogLevel.Information, logEntry.Level);
    }
}
// [END functions_pubsub_unit_test]
// [END functions_cloudevent_pubsub_unit_test]
