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

using Xunit;

[Collection(nameof(LoggingFixture))]
public class WriteLogEntryTest
{
    private readonly LoggingFixture _loggingFixture;

    public WriteLogEntryTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void WriteLogEntry()
    {
        string logId = $"logForWriteLogEntryTest{_loggingFixture.RandomName()}";
        string message = "Example log entry.";

        // Try creating log with log entry.
        var createLog = new WriteLogEntrySample();
        var log = createLog.WriteLogEntry(_loggingFixture.ProjectId, logId, message);
        _loggingFixture.LogsToDelete.Add(logId);
        Assert.Contains(message, log.TextPayload);
    }
}
