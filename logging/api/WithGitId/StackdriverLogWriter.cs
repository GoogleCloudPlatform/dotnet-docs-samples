/*
 * Copyright (c) 2017 Google Inc.
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

using Google.Api;
using Google.Cloud.Logging.V2;
using Google.Cloud.Logging.Type;
using Google.Cloud.DevTools.Source.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using Google.Protobuf;

namespace GoogleCloudSamples
{
    public static class StackdriverLogWriter
    {
        public static string LogId { get; set; } = "YOUR-LOG-ID";

        public static string ProjectId { get; set; } = "YOUR-PROJECT-ID";

        public static string WriteLog(
            string msg,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            var client = LoggingServiceV2Client.Create();

            var resource = new MonitoredResource { Type = "global" };
            resource.Labels["project_id"] = ProjectId;

            LogName logName = new LogName(ProjectId, LogId);
            LogEntrySourceLocation sourceLocation = new LogEntrySourceLocation
            {
                File = sourceFilePath,
                Line = sourceLineNumber,
                Function = memberName
            };

            LogEntry logEntry = new LogEntry
            {
                LogNameAsLogName = logName,
                Severity = LogSeverity.Info,
                SourceLocation = sourceLocation,
                TextPayload = msg
            };

            IDictionary<string, string> entryLabels = new Dictionary<string, string>
            {
                { "size", "large" },
                { "color", "red" }
            };

            TryAddGitRevisionId(entryLabels);

            client.WriteLogEntries(
                logName: logName,
                resource: resource,
                labels: entryLabels,
                entries: new[] { logEntry },
                callSettings: null);
            return logEntry.ToString();
        }

        private static void TryAddGitRevisionId(IDictionary<string, string> labels)
        {
            try
            {
                var gitId = SourceContext.AppSourceContext?.Git?.RevisionId ?? "";
                Debug.Assert(!String.IsNullOrWhiteSpace(gitId), "No valid git revision id found.",
                    "Make sure you have source-context.json published with the application.");
                labels.Add(SourceContext.GitRevisionIdLogLabel, gitId);
            }
            catch (Exception ex) when (
                ex is SecurityException
                || ex is InvalidProtocolBufferException
                || ex is InvalidJsonException
                || ex is UnauthorizedAccessException)
            {
                // This is best-effort only, exceptions from reading/parsing the source_context.json are ignored.
                Debug.Fail("Exception at TryAddGitRevisionId.", ex.ToString());
            }
        }
    }
}