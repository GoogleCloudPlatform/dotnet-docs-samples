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

using Google.Logging.V2;
using Google.Logging.Type;
using System;
using System.IO;
using System.Collections.Generic;
using Google.Api;
using Google.Api.Gax.Grpc;

namespace GoogleCloudSamples
{
    public class LoggingSample
    {
        private static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
                "Usage: \n" +
                "  LoggingSample create-log-entry log-id new-log-entry-text\n" +
                "  LoggingSample list-log-entries log-id\n" +
                "  LoggingSample create-sink sink-id log-id\n" +
                "  LoggingSample list-sinks\n" +
                "  LoggingSample update-sink sink-id log-id\n" +
                "  LoggingSample delete-log log-id\n" +
                "  LoggingSample delete-sink sink-id \n";

        private CallSettings RetryAWhile
        {
            get
            {
                return CallSettings.FromCallTiming(CallTiming.FromRetry(new RetrySettings(
                    new BackoffSettings(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), 2.0),
                    new BackoffSettings(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)),
                    Google.Api.Gax.Expiration.FromTimeout(TimeSpan.FromSeconds(30)),
                    (Grpc.Core.RpcException e) => e.Status.StatusCode == Grpc.Core.StatusCode.Internal
                    )));
            }
        }

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

        // [START write_log_entry]
        private void WriteLogEntry(string logId, string message)
        {
            var client = LoggingServiceV2Client.Create();
            LogEntry logEntry = new LogEntry
            {
                LogName = $"projects/{s_projectId}/logs/{logId}",
                Severity = LogSeverity.Info,
                TextPayload = $"{typeof(LoggingSample).FullName} - {message}"
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };
            client.WriteLogEntries(logEntry.LogName, resource, entryLabels,
                new[] { logEntry }, RetryAWhile);
            Console.WriteLine($"Created log entry in log-id: {logId}.");
        }
        // [END write_log_entry]

        // [START list_log_entries]
        private void ListLogEntries(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            string logName = $"projects/{s_projectId}/logs/{logId}";
            IEnumerable<string> projectIds = new string[] { s_projectId };
            var results = client.ListLogEntries(projectIds, $"logName={logName}",
                "timestamp desc", callSettings: RetryAWhile);
            foreach (var row in results)
            {
                if (row != null && !String.IsNullOrEmpty(row.TextPayload.Trim()))
                {
                    Console.WriteLine($"{row.TextPayload.Trim()}");
                }
                else
                {
                    results.GetEnumerator().Dispose();
                    break;
                }
            }
        }
        // [END list_log_entries]

        // [START create_log_sink]
        private void CreateSink(string sinkId, string logId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            CreateSinkRequest sinkRequest = new CreateSinkRequest();
            LogSink myLogSink = new LogSink();
            string sinkName = $"projects/{s_projectId}/sinks/{sinkId}";
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
            string logName = $"projects/{s_projectId}/logs/{logId}";
            myLogSink.Filter = $"logName={logName}AND severity<=ERROR";
            sinkRequest.Parent = $"projects/{s_projectId}";
            sinkRequest.Sink = myLogSink;
            sinkClient.CreateSink(sinkRequest.Parent, myLogSink, RetryAWhile);
            Console.WriteLine($"Created sink: {sinkId}.");
        }
        // [END create_log_sink]

        // [START list_log_sinks]
        private void ListSinks()
        {
            var sinkClient = ConfigServiceV2Client.Create();
            var listOfSinks = sinkClient.ListSinks($"projects/{s_projectId}",
                callSettings: RetryAWhile);
            foreach (var sink in listOfSinks)
            {
                Console.WriteLine($"{sink.Name} {sink.ToString()}");
            }
        }
        // [END list_log_sinks]

        // [START update_log_sink]
        private void UpdateSinkLog(string sinkId, string logId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            string logName = $"projects/{s_projectId}/logs/{logId}";
            string sinkName = $"projects/{s_projectId}/sinks/{sinkId}";
            var sink = sinkClient.GetSink(sinkName, RetryAWhile);
            sink.Filter = $"logName={logName}AND severity<=ERROR";
            sinkClient.UpdateSink(sinkName, sink, RetryAWhile);
            Console.WriteLine($"Updated {sinkId} to export logs from {logId}.");
        }
        // [END update_log_sink]

        // [START delete_log]
        private void DeleteLog(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            string logName = $"projects/{s_projectId}/logs/{logId}";
            client.DeleteLog(logName, RetryAWhile);
            Console.WriteLine($"Deleted {logId}.");
        }
        // [END delete_log]

        // [START delete_log_sink]
        private void DeleteSink(string sinkId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            string sinkName = $"projects/{s_projectId}/sinks/{sinkId}";
            sinkClient.DeleteSink(sinkName, RetryAWhile);
            Console.WriteLine($"Deleted {sinkId}.");
        }
        // [END delete_log_sink]

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
                        if (args.Length < 3 && PrintUsage()) return -1;
                        WriteLogEntry(args[1], args[2]);
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
