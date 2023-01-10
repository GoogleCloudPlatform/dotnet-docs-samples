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
using Google.Cloud.Functions.Testing;
using Google.Events;
using Google.Events.Protobuf.Cloud.Storage.V1;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests;

public class HelloGcsTest : FunctionTestBase<HelloGcs.Function>
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
            TimeCreated = new DateTimeOffset(2020, 7, 9, 13, 0, 5, TimeSpan.Zero).ToTimestamp(),
            Updated = new DateTimeOffset(2020, 7, 9, 13, 23, 25, TimeSpan.Zero).ToTimestamp()
        };
        var cloudEvent = new CloudEvent
        {
            Type = StorageObjectData.DeletedCloudEventType,
            Source = new Uri("//storage.googleapis.com", UriKind.RelativeOrAbsolute),
            Id = "1234",
            Data = data
        };

        await ExecuteCloudEventRequestAsync(cloudEvent);

        var logs = GetFunctionLogEntries();
        Assert.All(logs, entry => Assert.Equal(LogLevel.Information, entry.Level));

        var actualMessages = logs.Select(entry => entry.Message).ToArray();
        var expectedMessages = new[]
        {
            "Event: 1234",
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
