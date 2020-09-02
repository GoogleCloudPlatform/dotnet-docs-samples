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

using Google.Api.Gax.Grpc;
using Google.Cloud.Logging.V2;
using Grpc.Core;
using System;
using System.Collections.Generic;
using Xunit;

[CollectionDefinition(nameof(LoggingFixture))]
public class LoggingFixture : IDisposable, ICollectionFixture<LoggingFixture>
{
    public string ProjectId { get; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public List<string> LogsToDelete { get; } = new List<string>();
    public List<string> SinksToDelete { get; } = new List<string>();

    public string RandomName() => Guid.NewGuid().ToString();

    public void Dispose()
    {
        var exceptions = new List<Exception>();
        var deleteLog = new DeleteLogSample();
        var deleteSink = new DeleteSinkSample();

        // Delete all logs created from running the tests.
        foreach (string log in LogsToDelete)
        {
            try
            {
                deleteLog.DeleteLog(ProjectId, log);
            }
            catch (RpcException)
            {
                // Do nothing, we are deleting on a best effort basis.
            }
        }
        // Delete all the log sinks created from running the tests.
        foreach (string sink in SinksToDelete)
        {
            try
            {
                deleteSink.DeleteSink(ProjectId, sink);
            }
            catch (RpcException)
            {
                // Do nothing, we are deleting on a best effort basis.
            }
        }
    }

    public LogSink GetSink(string projectId, string sinkId)
    {
        var callSettings = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        var sinkClient = ConfigServiceV2Client.Create();
        LogSinkName logSinkName = LogSinkName.FromProjectSink(projectId, sinkId);

        return sinkClient.GetSink(logSinkName, callSettings);
    }

    public LogEntry CreateLog(string logId, string message)
    {
        var createLog = new WriteLogEntrySample();
        var log = createLog.WriteLogEntry(ProjectId, logId, message);
        LogsToDelete.Add(logId);
        return log;
    }
}
