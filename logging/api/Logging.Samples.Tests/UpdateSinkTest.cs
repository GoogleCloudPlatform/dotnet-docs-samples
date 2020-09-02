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

using Google.Cloud.Logging.V2;
using Xunit;

[Collection(nameof(LoggingFixture))]
public class UpdateSinkTest
{
    private readonly LoggingFixture _loggingFixture;

    public UpdateSinkTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void UpdateSink()
    {
        string randomName = _loggingFixture.RandomName();
        string sinkId = $"sinkForUpdateSinkTest{randomName}";
        string logId = $"logForUpdateSinkTest{randomName}";
        string newLogId = $"newlogForUpdateSinkTest{randomName}";
        string message = "Example log entry.";

        // Try creating logs with log entry.
        _loggingFixture.CreateLog(logId, message);

        _loggingFixture.CreateLog(newLogId, message);

        // Try creating sink.
        var createSink = new CreateSinkSample();
        var sink = createSink.CreateSink(_loggingFixture.ProjectId, sinkId, logId);
        _loggingFixture.SinksToDelete.Add(sinkId);

        // Try updateing sink
        var updateSink = new UpdateSinkSample();
        var updatedSink = updateSink.UpdateSink(_loggingFixture.ProjectId, sinkId, newLogId);

        Assert.Equal(updatedSink.Filter, 
            $"logName={LogName.FromProjectLog(_loggingFixture.ProjectId, newLogId)}AND severity<=ERROR");
    }
}
