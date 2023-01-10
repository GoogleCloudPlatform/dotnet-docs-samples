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

// [START functions_tips_infinite_retries]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimeBoundedRetries;

public class Function : ICloudEventFunction<MessagePublishedData>
{
    private static readonly TimeSpan MaxEventAge = TimeSpan.FromSeconds(10);
    private readonly ILogger _logger;

    // Note: for additional testability, use an injectable clock abstraction.
    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
    {
        string textData = data.Message.TextData;

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        // Every PubSub CloudEvent will contain a timestamp.
        DateTimeOffset timestamp = cloudEvent.Time.Value;
        DateTimeOffset expiry = timestamp + MaxEventAge;

        // Ignore events that are too old.
        if (utcNow > expiry)
        {
            _logger.LogInformation("Dropping PubSub message '{text}'", textData);
            return Task.CompletedTask;
        }

        // Process events that are recent enough.
        // If this processing throws an exception, the message will be retried until either
        // processing succeeds or the event becomes too old and is dropped by the code above.
        _logger.LogInformation("Processing PubSub message '{text}'", textData);
        return Task.CompletedTask;
    }
}
// [END functions_tips_infinite_retries]
