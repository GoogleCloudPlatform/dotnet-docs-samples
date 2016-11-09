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
using Google.Logging.V2;
using Xunit;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace GoogleCloudSamples
{
    public class BaseTest
    {
        private readonly string _projectId;

        public BaseTest()
        {
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        }

        public struct ConsoleOutput
        {
            public int ExitCode;
            public string Stdout;
        };

        /// <summary>Runs LoggingSample.exe with the provided arguments</summary>
        /// <returns>The console output of this program</returns>
        public static ConsoleOutput Run(params string[] arguments)
        {
            Console.Write("LoggingSample.exe ");
            Console.WriteLine(string.Join(" ", arguments));

            using (var output = new StringWriter())
            {
                LoggingSample loggingSample = new LoggingSample(output);
                var consoleOutput = new ConsoleOutput()
                {
                    ExitCode = loggingSample.Run(arguments),
                    Stdout = output.ToString()
                };
                Console.Write(consoleOutput.Stdout);
                return consoleOutput;
            }
        }

        protected static void AssertSucceeded(ConsoleOutput output)
        {
            Assert.True(0 == output.ExitCode,
                $"Exit code: {output.ExitCode}\n{output.Stdout}");
        }

        public class LoggingTest : BaseTest
        {
            [Fact]
            public void TestCreateLogEntry()
            {
                string logId = "logForTestCreateLogEntry";
                string message = "Example log entry.";
                // Try creating a log entry.
                var created = Run("create-log-entry", logId, message);
                AssertSucceeded(created);
                // Pause for 5 seconds before trying to get newly added log entry.
                Thread.Sleep(5000);
                // Retrieve the log entry just added, using the logId as a filter.
                var results = Run("list-log-entries", logId);
                // Confirm returned log entry contains expected value.
                Assert.Contains(message, results.Stdout);
                Run("delete-log", logId);
            }

            [Fact]
            public void TestListEntries()
            {
                string logId = "logForTestListEntries";
                string message1 = "Example log entry.";
                string message2 = "Another example log entry.";
                string message3 = "Additional example log entry.";
                // Try creating three log entries.
                var created1 = Run("create-log-entry", logId, message1);
                AssertSucceeded(created1);
                var created2 = Run("create-log-entry", logId, message2);
                AssertSucceeded(created2);
                var created3 = Run("create-log-entry", logId, message3);
                AssertSucceeded(created3);
                // Pause for 5 seconds before trying to get newly added log entries.
                Thread.Sleep(5000);
                // Retrieve the log entries just added, using the logId as a filter.
                var results = Run("list-log-entries", logId);
                // Confirm returned log entry contains expected value.
                Assert.Contains(message3, results.Stdout);
                Run("delete-log", logId);
            }

            [Fact]
            public void TestDeleteLog()
            {
                string logId = "logForTestDeleteLog";
                string message = "Example log entry.";
                //Try creating a log entry
                var created = Run("create-log-entry", logId, message);
                AssertSucceeded(created);
                // Pause for 5 seconds before trying to get newly added log entry.
                Thread.Sleep(5000);
                // Retrieve the log entry just added, using the logId as a filter.
                var results = Run("list-log-entries", logId);
                // Confirm returned log entry contains expected value.
                Assert.Contains(message, results.Stdout);
                // Try deleting log.
                Run("delete-log", logId);
                // Pause for 5 seconds before trying to list logs from deleted log.
                Thread.Sleep(5000);
                // Try listing the log entries.  There should be none.
                var listed = Run("list-log-entries", logId);
                AssertSucceeded(listed);
                Assert.Equal("", listed.Stdout.Trim());
            }

            [Fact]
            public void TestCreateSink()
            {
                string sinkId = "sinkForTestCreateSink";
                string logId = "logForTestCreateSink";
                string sinkName = $"projects/{_projectId}/sinks/{sinkId}";
                // Try creating sink.
                var created = Run("create-sink", sinkId, logId);
                AssertSucceeded(created);
                var sinkClient = ConfigServiceV2Client.Create();
                var results = sinkClient.GetSink(sinkName);
                // Confirm newly created sink is returned.
                Assert.NotNull(results);
                Run("delete-sink", sinkName);
            }

            [Fact]
            public void TestListSinks()
            {
                string sinkId = "sinkForTestListSinks";
                string logId = "logForTestListSinks";
                string sinkName = $"projects/{_projectId}/sinks/{sinkId}";
                // Try creating sink.
                var created = Run("create-sink", sinkId, logId);
                AssertSucceeded(created);
                // Try listing sinks.
                var results = Run("list-sinks");
                // Confirm list-sinks results are not null.
                Assert.NotNull(results);
                Run("delete-sink", sinkName);
            }

            [Fact]
            public void TestUpdateSink()
            {
                string sinkId = "sinkForTestUpdateSink";
                string logId = "logForTestUpdateSink";
                string newLogId = "newlogForTestUpdateSink";
                string sinkName = $"projects/{_projectId}/sinks/{sinkId}";
                // Try creating sink.
                var created = Run("create-sink", sinkId, logId);
                AssertSucceeded(created);
                // Try updating sink.
                var updated = Run("update-sink", sinkId, newLogId);
                AssertSucceeded(updated);
                // Get sink to confirm that log has been updated.
                var sinkClient = ConfigServiceV2Client.Create();
                var results = sinkClient.GetSink(sinkName);
                var currentLog = results.Filter;
                Assert.Contains(newLogId, currentLog);
                Run("delete-sink", sinkName);
            }

            [Fact]
            public void TestDeleteSink()
            {
                string sinkId = "sinkForTestDeleteSink";
                string logId = "logForTestDeleteSink";
                string sinkName = $"projects/{_projectId}/sinks/{sinkId}";
                // Try creating sink.
                var created = Run("create-sink", sinkId, logId);
                AssertSucceeded(created);
                // Try deleting sink.
                Run("delete-sink", sinkName);
                // Get sink to confirm it has been deleted.
                var sinkClient = ConfigServiceV2Client.Create();
                Exception ex = Assert.Throws<Grpc.Core.RpcException>(() =>
                    sinkClient.GetSink(sinkName));
            }

            private string GetConsoleAppOutput(string filePath)
            {
                string output;
                Process consoleApp = new Process();
                consoleApp.StartInfo.FileName = filePath;
                consoleApp.StartInfo.UseShellExecute = false;
                consoleApp.StartInfo.RedirectStandardOutput = true;
                consoleApp.Start();
                output = consoleApp.StandardOutput.ReadToEnd();
                consoleApp.WaitForExit();
                return output;
            }

            [Fact]
            public void TestQuickStartConsoleApp()
            {
                string output;
                string filePath = @"..\..\..\QuickStart\bin\Debug\QuickStart.exe";
                string expectedOutput = "Log Entry created.";
                // This logId should match the logId value set in QuickStart\QuickStart.cs
                string logId = "my-log";
                string message = "Hello World!";
                output = GetConsoleAppOutput(filePath).Trim();
                Assert.Equal(expectedOutput, output);
                // Pause for 5 seconds before trying to get newly added log entry.
                Thread.Sleep(5000);
                // Retrieve the log entry just added, using the logId as a filter.
                var results = Run("list-log-entries", logId);
                // Confirm returned log entry contains expected value.
                Assert.Contains(message, results.Stdout);
                Run("delete-log", logId);
            }
        }
    }
}
