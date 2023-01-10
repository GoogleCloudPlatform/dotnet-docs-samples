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

// [START functions_cloudevent_storage_unit_test]
// [START functions_storage_unit_test]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Testing;
using Google.Events;
using Google.Events.Protobuf.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorld.Tests;

public class HelloGcsUnitTest
{
    [Fact]
    public async Task FileNameIsLogged()
    {
        // Prepare the inputs
        var data = new StorageObjectData { Name = "new-file.txt" };
        var cloudEvent = new CloudEvent
        {
            Type = StorageObjectData.FinalizedCloudEventType,
            Source = new Uri("//storage.googleapis.com", UriKind.RelativeOrAbsolute),
            Id = "1234",
            Data = data
        };
        var logger = new MemoryLogger<HelloGcs.Function>();

        // Execute the function
        var function = new HelloGcs.Function(logger);
        await function.HandleAsync(cloudEvent, data, CancellationToken.None);

        // Check the log results - just the entry starting with "File:".
        var logEntry = Assert.Single(logger.ListLogEntries(), entry => entry.Message.StartsWith("File:"));
        Assert.Equal("File: new-file.txt", logEntry.Message);
        Assert.Equal(LogLevel.Information, logEntry.Level);
    }
}
// [END functions_storage_unit_test]
// [END functions_cloudevent_storage_unit_test]
