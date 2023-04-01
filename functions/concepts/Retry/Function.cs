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

// [START functions_tips_retry]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Retry;

public class Function : ICloudEventFunction<MessagePublishedData>
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
    {
        bool retry = false;
        string text = data.Message?.TextData;

        // Get the value of the "retry" JSON parameter, if one exists.
        if (!string.IsNullOrEmpty(text))
        {
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(data.Message.TextData);

            retry = element.TryGetProperty("retry", out var property) &&
                property.ValueKind == JsonValueKind.True;
        }

        // Throwing an exception causes the execution to be retried.
        if (retry)
        {
            throw new InvalidOperationException("Retrying...");
        }
        else
        {
            _logger.LogInformation("Not retrying...");
        }
        return Task.CompletedTask;
    }
}
// [END functions_tips_retry]
