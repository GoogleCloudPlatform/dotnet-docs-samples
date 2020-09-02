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

// [START logging_write_log_entry]

using Google.Api;
using Google.Api.Gax.Grpc;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Grpc.Core;
using System;
using System.Collections.Generic;

public class WriteLogEntrySample
{
    public LogEntry WriteLogEntry(string projectId, string logId, string message)
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
        LogEntry logEntry = new LogEntry
        {
            LogNameAsLogName = logName,
            Severity = LogSeverity.Info,
            TextPayload = $"{typeof(WriteLogEntrySample).FullName} - {message}"
        };
        MonitoredResource resource = new MonitoredResource { Type = "global" };
        IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };
        client.WriteLogEntries(logName, resource, entryLabels, new[] { logEntry }, callSettings);

        return logEntry;
    }
}
// [END logging_write_log_entry]
