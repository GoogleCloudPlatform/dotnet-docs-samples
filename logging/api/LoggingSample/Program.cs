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

using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;
using System;
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
                "  dotnet run create-log-entry log-id new-log-entry-text\n" +
                "  dotnet run list-log-entries log-id\n" +
                "  dotnet run create-sink sink-id log-id\n" +
                "  dotnet run list-sinks\n" +
                "  dotnet run update-sink sink-id log-id\n" +
                "  dotnet run delete-log log-id\n" +
                "  dotnet run delete-sink sink-id \n";

        private CallSettings RetryAWhile
        {
            get
            {
                return CallSettings.FromCallTiming(CallTiming.FromRetry(new RetrySettings(
                    new BackoffSettings(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), 2.0),
                    new BackoffSettings(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)),
                    Google.Api.Gax.Expiration.FromTimeout(TimeSpan.FromSeconds(90)),
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
            LogName logName = new LogName(s_projectId, logId);
            LogEntry logEntry = new LogEntry
            {
                LogName = logName.ToString(),
                Severity = LogSeverity.Info,
                TextPayload = $"{typeof(LoggingSample).FullName} - {message}"
            };
            MonitoredResource resource = new MonitoredResource { Type = "global" };
            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };
            client.WriteLogEntries(LogNameOneof.From(logName), resource, entryLabels,
                new[] { logEntry }, RetryAWhile);
            Console.WriteLine($"Created log entry in log-id: {logId}.");
        }
        // [END write_log_entry]

        // [START list_log_entries]
        private void ListLogEntries(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            ProjectName projectName = new ProjectName(s_projectId);
            IEnumerable<string> projectIds = new string[] { projectName.ToString() };
            var results = client.ListLogEntries(projectIds, $"logName={logName.ToString()}",
                "timestamp desc", callSettings: RetryAWhile);
            foreach (var row in results)
            {
                Console.WriteLine($"{row.TextPayload.Trim()}");
            }
        }
        // [END list_log_entries]

        // [START create_log_sink]
        private void CreateSink(string sinkId, string logId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            CreateSinkRequest sinkRequest = new CreateSinkRequest();
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
            sinkRequest.Sink = myLogSink;
            sinkClient.CreateSink(ParentNameOneof.From(projectName), myLogSink, RetryAWhile);
            Console.WriteLine($"Created sink: {sinkId}.");
        }
        // [END create_log_sink]

        // [START list_log_sinks]
        private void ListSinks()
        {
            var sinkClient = ConfigServiceV2Client.Create();
            ProjectName projectName = new ProjectName(s_projectId);
            var listOfSinks = sinkClient.ListSinks(ParentNameOneof.From(projectName),
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
            LogName logName = new LogName(s_projectId, logId);
            SinkName sinkName = new SinkName(s_projectId, sinkId);
            var sink = sinkClient.GetSink(SinkNameOneof.From(sinkName), RetryAWhile);
            sink.Filter = $"logName={logName.ToString()}AND severity<=ERROR";
            sinkClient.UpdateSink(SinkNameOneof.From(sinkName), sink, RetryAWhile);
            Console.WriteLine($"Updated {sinkId} to export logs from {logId}.");
        }
        // [END update_log_sink]

        // [START delete_log]
        private void DeleteLog(string logId)
        {
            var client = LoggingServiceV2Client.Create();
            LogName logName = new LogName(s_projectId, logId);
            client.DeleteLog(LogNameOneof.From(logName), RetryAWhile);
            Console.WriteLine($"Deleted {logId}.");
        }
        // [END delete_log]

        // [START delete_log_sink]
        private void DeleteSink(string sinkId)
        {
            var sinkClient = ConfigServiceV2Client.Create();
            SinkName sinkName = new SinkName(s_projectId, sinkId);
            sinkClient.DeleteSink(SinkNameOneof.From(sinkName), RetryAWhile);
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
