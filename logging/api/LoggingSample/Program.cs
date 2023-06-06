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
// [START complete]

using Google.Api;
using Google.Api.Gax.Grpc;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloudSamples
{
    public class LoggingSample
    {
        private static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
                "Usage: \n" +
                "  dotnet run create-log-entry log-id\n" +
                "  dotnet run list-log-entries log-id\n" +
                "  dotnet run create-sink sink-id log-id\n" +
                "  dotnet run list-sinks\n" +
                "  dotnet run update-sink sink-id log-id\n" +
                "  dotnet run delete-log log-id\n" +
                "  dotnet run delete-sink sink-id \n";

        private readonly CallSettings _retryAWhile = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        public bool PrintUsage()
        {
            Console.WriteLine(s_usage);
            return true;
        }

        public static int Main(string[] args)
        {
            if (s_projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Update Program.cs and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.");
                return -1;
            }
            LoggingSample loggingSample = new LoggingSample();
            return loggingSample.Run(args);
        }

        // [START logging_write_log_entry]
        private void WriteLogEntry(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            var jsonPayload = new Struct()
            {
                Fields =
                {
                    { "name", Value.ForString("King Arthur") },
                    { "quest", Value.ForString("Find the Holy Grail") },
                    { "favorite_color", Value.ForString("Blue") }
                }
            };
            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = LogSeverity.Info,
                JsonPayload = jsonPayload
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "blue" }
            };
            client.WriteLogEntries(logName, resource, entryLabels,
                new[] { logEntry }, _retryAWhile);
            Console.WriteLine($"Created log entry in log-id: {logId}.");
        }
        // [END logging_write_log_entry]

        // [START logging_list_log_entries]
        private void ListLogEntries(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            ProjectName projectName = new ProjectName(s_projectId);
            var results = client.ListLogEntries(Enumerable.Repeat(projectName, 1), $"logName={logName.ToString()}",
                "timestamp desc", callSettings: _retryAWhile);
            foreach (var row in results)
            {
                Console.WriteLine($"{row.TextPayload.Trim()}");
            }
        }
        // [END logging_list_log_entries]

        // [START logging_create_sink]
        private void CreateSink(string sinkId, string logId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            LogSink myLogSink = new LogSink();
            myLogSink.Name = sinkId;

            // This creates a sink using a Google Cloud Storage bucket 
            // named the same as the projectId.
            // This requires editing the bucket's permissions to add the Entity Group 
            // named 'cloud-logs@google.com' with 'Owner' access for the bucket.
            // If this is being run with a Google Cloud service account,
            // that account will need to be granted 'Owner' access to the Project.
            // In Powershell, use this command:
            // PS > Add-GcsBucketAcl <your-bucket-name> -Role OWNER -Group cloud-logs@google.com
            myLogSink.Destination = "storage.googleapis.com/" + s_projectId;
            LogName logName = new LogName(s_projectId, logId);
            myLogSink.Filter = $"logName={logName.ToString()}AND severity<=ERROR";
            ProjectName projectName = new ProjectName(s_projectId);
            sinkClient.CreateSink(projectName, myLogSink, _retryAWhile);
            Console.WriteLine($"Created sink: {sinkId}.");
        }
        // [END logging_create_sink]

        // [START logging_list_sinks]
        private void ListSinks()
        {
            var sinkClient = ConfigServiceV2Client.Create();
            ProjectName projectName = new ProjectName(s_projectId);
            var listOfSinks = sinkClient.ListSinks(projectName, callSettings: _retryAWhile);
            foreach (var sink in listOfSinks)
            {
                Console.WriteLine($"{sink.Name} {sink.ToString()}");
            }
        }
        // [END logging_list_sinks]

        // [START logging_update_sink]
        private void UpdateSinkLog(string sinkId, string logId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            LogSinkName sinkName = new LogSinkName(s_projectId, sinkId);
            var sink = sinkClient.GetSink(sinkName, _retryAWhile);
            sink.Filter = $"logName={logName.ToString()}AND severity<=ERROR";
            sinkClient.UpdateSink(sinkName, sink, _retryAWhile);
            Console.WriteLine($"Updated {sinkId} to export logs from {logId}.");
        }
        // [END logging_update_sink]

        // [START logging_delete_log]
        private void DeleteLog(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            client.DeleteLog(logName, _retryAWhile);
            Console.WriteLine($"Deleted {logId}.");
        }
        // [END logging_delete_log]

        // [START logging_delete_sink]
        private void DeleteSink(string sinkId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            LogSinkName sinkName = new LogSinkName(s_projectId, sinkId);
            sinkClient.DeleteSink(sinkName, _retryAWhile);
            Console.WriteLine($"Deleted {sinkId}.");
        }
        // [END logging_delete_sink]

        public int Run(string[] args)
        {
            if (s_projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Update Program.cs and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.");
                return -1;
            }
            if (args.Length < 1 && PrintUsage()) return -1;
            try
            {
                switch (args[0].ToLower())
                {
                    case "create-log-entry":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        WriteLogEntry(args[1]);
                        break;
                    case "list-log-entries":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        ListLogEntries(args[1]);
                        break;
                    case "create-sink":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        CreateSink(args[1], args[2]);
                        break;
                    case "list-sinks":
                        ListSinks();
                        break;
                    case "update-sink":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        UpdateSinkLog(args[1], args[2]);
                        break;
                    case "delete-log":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DeleteLog(args[1]);
                        break;
                    case "delete-sink":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DeleteSink(args[1]);
                        break;
                    default:
                        PrintUsage();
                        return -1;
                }
                return 0;
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
        }
    }
}
// [END complete]
