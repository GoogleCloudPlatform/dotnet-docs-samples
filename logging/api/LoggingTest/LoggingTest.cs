/*
 * Copyright (c) 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using Google.Cloud.Logging.V2;
using Xunit;
using System.Diagnostics;
using System.Collections.Generic;
using Grpc.Core;
using System.Linq;

namespace GoogleCloudSamples
{
    public class BaseTest
    {
        private readonly string _projectId;
        private readonly List<string> _logsToDelete = new List<string>();
        private readonly List<string> _sinksToDelete = new List<string>();
        private readonly CommandLineRunner _runner = new CommandLineRunner()
        {
            Command = "LoggingSample",
            Main = LoggingSample.Main
        };
        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            ShouldRetry = (Exception e) =>
            {
                if (e is Xunit.Sdk.XunitException)
                    return true;
                var rpcException = e as RpcException;
                if (rpcException != null)
                {
                    return new[] { StatusCode.Aborted, StatusCode.Internal,
                        StatusCode.Cancelled }
                        .Contains(rpcException.Status.StatusCode);
                }
                return false;
            },
            DelayMultiplier = 3
        };

        public BaseTest()
        {
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        }

        /// <summary>Runs LoggingSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public ConsoleOutput Run(params string[] arguments) =>
            _runner.Run(arguments);

        public void Eventually(Action a) => _retryRobot.Eventually(a);

        public class LoggingTest : BaseTest, IDisposable
        {
            public void Dispose()
            {
                try
                {
                    // Delete all logs created from running the tests.
                    foreach (string log in _logsToDelete)
                    {
                        Run("delete-log", log);
                    }
                }
                catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
                try
                {
                    // Delete all the log sinks created from running the tests.
                    foreach (string sink in _sinksToDelete)
                    {
                        Run("delete-sink", sink);
                    }
                }
                catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
            }

            [Fact]
            public void TestCreateLogEntry()
            {
                string logId = "logForTestCreateLogEntry";
                string message = "Example log entry.";
                _logsToDelete.Add(logId);
                // Try creating a log entry.
                var created = Run("create-log-entry", logId, message);
                created.AssertSucceeded();
                Eventually(() =>
                {
                    // Retrieve the log entry just added, using the logId as a filter.
                    var results = Run("list-log-entries", logId);
                    // Confirm returned log entry contains expected value.
                    Assert.Contains(message, results.Stdout);
                });
            }

            [Fact]
            public void TestListEntries()
            {
                string logId = "logForTestListEntries";
                string message1 = "Example log entry.";
                string message2 = "Another example log entry.";
                string message3 = "Additional example log entry.";
                _logsToDelete.Add(logId);
                // Try creating three log entries.
                Run("create-log-entry", logId, message1).AssertSucceeded();
                Run("create-log-entry", logId, message2).AssertSucceeded();
                Run("create-log-entry", logId, message3).AssertSucceeded();
                Eventually(() =>
                {
                    // Retrieve the log entries just added, using the logId as a filter.
                    var results = Run("list-log-entries", logId);
                    // Confirm returned log entry contains expected value.
                    Assert.Contains(message3, results.Stdout);
                });
            }

            [Fact]
            public void TestWithLogId()
            {
                StackdriverLogWriter.ProjectId = _projectId;
                StackdriverLogWriter.LogId = "TestWithLogId";
                string message1 = "TestWithLogId test example";
                _logsToDelete.Add(StackdriverLogWriter.LogId);
                StackdriverLogWriter.WriteLog("TestWithLogId test example");
                Eventually(() =>
                {
                    // Retrieve the log entries just added, using the logId as a filter.
                    var results = Run("list-log-entries", StackdriverLogWriter.LogId);
                    // Confirm returned log entry contains expected value.
                    Assert.Contains(message1, results.Stdout);
                });
            }

            [Fact]
            public void TestDeleteLog()
            {
                string logId = "logForTestDeleteLog";
                string message = "Example log entry.";
                //Try creating a log entry
                var created = Run("create-log-entry", logId, message);
                created.AssertSucceeded();
                // Try deleting log and assert on success.
                Run("delete-log", logId).AssertSucceeded();
            }

            [Fact]
            public void TestCreateSink()
            {
                string sinkId = "sinkForTestCreateSink";
                string logId = "logForTestCreateSink";
                SinkName sinkName = new SinkName(_projectId, sinkId);
                string message = "Example log entry.";
                _sinksToDelete.Add(sinkId);
                _logsToDelete.Add(logId);
                // Try creating log with log entry.
                var created1 = Run("create-log-entry", logId, message);
                created1.AssertSucceeded();
                // Try creating sink.
                var created2 = Run("create-sink", sinkId, logId);
                created2.AssertSucceeded();
                var sinkClient = ConfigServiceV2Client.Create();
                var results = sinkClient.GetSink(SinkNameOneof.From(sinkName));
                // Confirm newly created sink is returned.
                Assert.NotNull(results);
            }

            [Fact]
            public void TestListSinks()
            {
                string sinkId = "sinkForTestListSinks";
                string logId = "logForTestListSinks";
                string sinkName = $"projects/{_projectId}/sinks/{sinkId}";
                string message = "Example log entry.";
                _logsToDelete.Add(logId);
                _sinksToDelete.Add(sinkId);
                // Try creating log with log entry.
                var created1 = Run("create-log-entry", logId, message);
                created1.AssertSucceeded();
                // Try creating sink.
                var created2 = Run("create-sink", sinkId, logId);
                created2.AssertSucceeded();
                Eventually(() =>
                {
                    // Try listing sinks.
                    var results = Run("list-sinks");
                    // Confirm list-sinks results are not null.
                    Assert.NotNull(results);
                });
            }

            [Fact]
            public void TestUpdateSink()
            {
                string sinkId = "sinkForTestUpdateSink";
                string logId = "logForTestUpdateSink";
                string newLogId = "newlogForTestUpdateSink";
                SinkName sinkName = new SinkName(_projectId, sinkId);
                string message = "Example log entry.";
                _sinksToDelete.Add(sinkId);
                _logsToDelete.Add(logId);
                _logsToDelete.Add(newLogId);
                // Try creating logs with log entries.
                Run("create-log-entry", logId, message).AssertSucceeded();
                Run("create-log-entry", newLogId, message).AssertSucceeded();
                Run("create-sink", sinkId, logId).AssertSucceeded();
                // Try updating sink.
                Run("update-sink", sinkId, newLogId).AssertSucceeded();
                // Get sink to confirm that log has been updated.
                var sinkClient = ConfigServiceV2Client.Create();
                var results = sinkClient.GetSink(SinkNameOneof.From(sinkName));
                var currentLog = results.Filter;
                Assert.Contains(newLogId, currentLog);
            }

            [Fact]
            public void TestDeleteSink()
            {
                string sinkId = "sinkForTestDeleteSink";
                string logId = "logForTestDeleteSink";
                SinkName sinkName = new SinkName(_projectId, sinkId);
                string message = "Example log entry.";
                _logsToDelete.Add(logId);
                // Try creating log with log entry.
                Run("create-log-entry", logId, message).AssertSucceeded();
                // Try creating sink.
                Run("create-sink", sinkId, logId).AssertSucceeded();
                // Try deleting sink.
                Run("delete-sink", sinkId);
                // Get sink to confirm it has been deleted.
                var sinkClient = ConfigServiceV2Client.Create();
                Exception ex = Assert.Throws<Grpc.Core.RpcException>(() =>
                    sinkClient.GetSink(SinkNameOneof.From(sinkName)));
            }

            readonly CommandLineRunner _quickStart = new CommandLineRunner()
            {
                VoidMain = QuickStart.Main,
                Command = "dotnet run"
            };

            [Fact]
            public void TestRunQuickStart()
            {
                string expectedOutput = "Log Entry created.";
                // This logId should match the logId value set in QuickStart\QuickStart.cs
                string logId = "my-log";
                string message = "Hello World!";
                _logsToDelete.Add(logId);
                var output = _quickStart.Run();
                Assert.Equal(expectedOutput, output.Stdout.Trim());
                Eventually(() =>
                {
                    // Retrieve the log entry just added, using the logId as a filter.
                    var results = Run("list-log-entries", logId);
                    // Confirm returned log entry contains expected value.
                    Assert.Contains(message, results.Stdout);
                });
            }
        }
    }
}
