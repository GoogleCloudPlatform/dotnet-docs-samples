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

using Grpc.Core;
using System;
using System.Linq;
using Xunit;

[Collection(nameof(LoggingFixture))]
public class DeleteLogTest
{
    private readonly LoggingFixture _loggingFixture;

    public DeleteLogTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void DeleteLog()
    {
        string logId = $"logForDeleteLogTest{_loggingFixture.RandomName()}";
        string message = "Example log entry.";

        // Try creating log with log entry.
        var createLog = new WriteLogEntrySample();
        var log = createLog.WriteLogEntry(_loggingFixture.ProjectId, logId, message);

        // Try deleteing log
        var deleteLog = new DeleteLogSample();
        deleteLog.DeleteLog(_loggingFixture.ProjectId, logId);

        // Try fetching a list of logs
        var listLogEntries = new ListLogEntriesSample();
        
        var logs = listLogEntries.ListLogEntries(_loggingFixture.ProjectId, logId).ToList();
        Assert.Empty(logs);

        // or it might thow some kind on an error 
        var exception = Assert.Throws<RpcException>(() => listLogEntries.ListLogEntries(_loggingFixture.ProjectId, logId).ToList());
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }
}
