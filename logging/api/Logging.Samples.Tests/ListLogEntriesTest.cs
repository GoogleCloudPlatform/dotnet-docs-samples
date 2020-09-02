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

using System.Linq;
using Xunit;

[Collection(nameof(LoggingFixture))]
public class ListLogEntriesTest
{
    private readonly LoggingFixture _loggingFixture;

    public ListLogEntriesTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void ListLogEntries()
    {
        string logId = $"logForListLogEntriesTest{_loggingFixture.RandomName()}";
        string message = "Example log entry.";

        // Try creating log with log entry.
        _loggingFixture.CreateLog(logId, message);

        // Try fetching a list of logs
        var listLogEntries = new ListLogEntriesSample();
        var logs = listLogEntries.ListLogEntries(_loggingFixture.ProjectId, logId).ToList();
        Assert.True(logs.Count > 0);
        Assert.Contains(logs, log => log.LogNameAsLogName.LogId == logId);
    }
}
