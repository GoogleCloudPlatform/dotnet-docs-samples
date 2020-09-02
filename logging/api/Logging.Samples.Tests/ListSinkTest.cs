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

using Google.Apis.Util;
using Google.Cloud.Logging.V2;
using System.Linq;
using Xunit;

[Collection(nameof(LoggingFixture))]
public class ListSinkTest
{
    private readonly LoggingFixture _loggingFixture;

    public ListSinkTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void ListSinks()
    {
        string randomName = _loggingFixture.RandomName();
        string sinkId = $"sinkForListSinksTest{randomName}";
        string logId = $"logForListSinksTest{randomName}";
        string message = "Example log entry.";
        LogSinkName sinkName = LogSinkName.FromProjectSink(_loggingFixture.ProjectId, sinkId);

        // Try creating log with log entry.
        _loggingFixture.CreateLog(logId, message);

        // Try creating sink.
        var createSink = new CreateSinkSample();
        var sink = createSink.CreateSink(_loggingFixture.ProjectId, sinkId, logId);
        _loggingFixture.SinksToDelete.Add(sinkId);

        // Try fetching a list of sinks
        var listSinks = new ListSinkSample();
        var sinks = listSinks.ListSinks(_loggingFixture.ProjectId);
        Assert.True(sinks.Count > 0);
        Assert.Contains(sinks, sink => sink.LogSinkName == sinkName);
    }
}
