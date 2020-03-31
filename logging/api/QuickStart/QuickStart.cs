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
// [START logging_quickstart]

using System;
// Imports the Google Cloud Logging client library
using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;
using System.Collections.Generic;
using Google.Api;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            // Instantiates a client.
            var client = LoggingServiceV2Client.Create();

            // Prepare new log entry.
            LogEntry logEntry = new LogEntry();
            string logId = "my-log";
            LogName logName = new LogName(projectId, logId);
            logEntry.LogNameAsLogName = logName;
            logEntry.Severity = LogSeverity.Info;

            // Create log entry message.
            string message = "Hello World!";
            string messageId = DateTime.Now.Millisecond.ToString();
            Type myType = typeof(QuickStart);
            string entrySeverity = logEntry.Severity.ToString().ToUpper();
            logEntry.TextPayload =
                $"{messageId} {entrySeverity} {myType.Namespace}.LoggingSample - {message}";

            // Set the resource type to control which GCP resource the log entry belongs to.
            // See the list of resource types at:
            // https://cloud.google.com/logging/docs/api/v2/resource-list
            // This sample uses resource type 'global' causing log entries to appear in the 
            // "Global" resource list of the Developers Console Logs Viewer:
            //  https://console.cloud.google.com/logs/viewer
            MonitoredResource resource = new MonitoredResource
            {
                Type = "global"
            };

            // Create dictionary object to add custom labels to the log entry.
            IDictionary<string, string> entryLabels = new Dictionary<string, string>();
            entryLabels.Add("size", "large");
            entryLabels.Add("color", "red");

            // Add log entry to collection for writing. Multiple log entries can be added.
            IEnumerable<LogEntry> logEntries = new LogEntry[] { logEntry };

            // Write new log entry.
            client.WriteLogEntries(logName, resource, entryLabels, logEntries);

            Console.WriteLine("Log Entry created.");
        }
    }
}
// [END logging_quickstart]
