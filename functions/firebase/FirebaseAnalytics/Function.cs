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

// [START functions_firebase_analytics]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Firebase.Analytics.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseAnalytics;

public class Function : ICloudEventFunction<AnalyticsLogData>
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public Task HandleAsync(CloudEvent cloudEvent, AnalyticsLogData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event source: {source}", cloudEvent.Source);
        _logger.LogInformation("Event count: {count}", data.EventDim.Count);

        var firstEvent = data.EventDim.FirstOrDefault();
        if (firstEvent is object)
        {
            _logger.LogInformation("First event name: {name}", firstEvent.Name);
            DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeMilliseconds(firstEvent.TimestampMicros / 1000);
            _logger.LogInformation("First event timestamp: {timestamp:u}", timestamp);
        }

        var userObject = data.UserDim;
        if (userObject is object)
        {
            _logger.LogInformation("Device model: {device}", userObject.DeviceInfo?.DeviceModel);
            _logger.LogInformation("Location: {city}, {country}", userObject.GeoInfo?.City, userObject.GeoInfo.Country);
        }
        // In this example, we don't need to perform any asynchronous operations, so the
        // method doesn't need to be declared async.
        return Task.CompletedTask;
    }
}
// [END functions_firebase_analytics]
