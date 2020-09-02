// Copyright 2020 Google Inc.
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

// [START logging_delete_log]

using Google.Api.Gax.Grpc;
using Google.Cloud.Logging.V2;
using Grpc.Core;
using System;

public class DeleteLogSample
{
    public void DeleteLog(string projectId, string logId)
    {
        var callSettings = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        var client = LoggingServiceV2Client.Create();
        LogName logName = LogName.FromProjectLog(projectId, logId);
        client.DeleteLog(logName, callSettings);
        Console.WriteLine($"Deleted Log {logId}.");
    }
}
// [END logging_delete_log]
