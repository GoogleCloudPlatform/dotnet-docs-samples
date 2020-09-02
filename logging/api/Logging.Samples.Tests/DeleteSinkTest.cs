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
using Xunit;

[Collection(nameof(LoggingFixture))]
public class DeleteSinkTest
{
    private readonly LoggingFixture _loggingFixture;

    public DeleteSinkTest(LoggingFixture loggingFixture)
    {
        _loggingFixture = loggingFixture;
    }

    [Fact]
    public void DeleteSink()
    {
        string randomName = _loggingFixture.RandomName();
        string sinkId = $"sinkForDeleteSinkTest{randomName}";
        string logId = $"logForDeleteSinkTest{randomName}";
        string message = "Example log entry.";

        // Try creating log with log entry.
        _loggingFixture.CreateLog(logId, message);

        // Try creating sink.
        var createSink = new CreateSinkSample();
        var sink = createSink.CreateSink(_loggingFixture.ProjectId, sinkId, logId);

        // Try deleteing sink
        var deleteSink = new DeleteSinkSample();
        deleteSink.DeleteSink(_loggingFixture.ProjectId, sinkId);

        var exception = Assert.Throws<RpcException>(() => _loggingFixture.GetSink(_loggingFixture.ProjectId, sinkId));
        Assert.Equal(StatusCode.NotFound, exception.StatusCode);
    }
}
