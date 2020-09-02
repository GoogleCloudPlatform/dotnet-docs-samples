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

// [START logging_create_sink]

using Google.Api.Gax.Grpc;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Logging.V2;
using Grpc.Core;
using System;

public class CreateSinkSample
{
    public LogSink CreateSink(string projectId, string sinkId, string logId)
    {
        var callSettings = CallSettings.FromRetry(
            RetrySettings.FromExponentialBackoff(
                maxAttempts: 15,
                initialBackoff: TimeSpan.FromSeconds(3),
                maxBackoff: TimeSpan.FromSeconds(12),
                backoffMultiplier: 2.0,
                retryFilter: RetrySettings.FilterForStatusCodes(StatusCode.Internal, StatusCode.DeadlineExceeded)));

        var sinkClient = ConfigServiceV2Client.Create();
        LogSink myLogSink = new LogSink
        {
            Name = sinkId,

            // This creates a sink using a Google Cloud Storage bucket 
            // named the same as the projectId.
            // This requires editing the bucket's permissions to add the Entity Group 
            // named 'cloud-logs@google.com' with 'Owner' access for the bucket.
            // If this is being run with a Google Cloud service account,
            // that account will need to be granted 'Owner' access to the Project.
            // In Powershell, use this command:
            // PS > Add-GcsBucketAcl <your-bucket-name> -Role OWNER -Group cloud-logs@google.com
            Destination = $"storage.googleapis.com/{projectId}"
        };
        LogName logName = LogName.FromProjectLog(projectId, logId);
        myLogSink.Filter = $"logName={logName}AND severity<=ERROR";
        ProjectName projectName = ProjectName.FromProject(projectId);
        var logSink = sinkClient.CreateSink(projectName, myLogSink, callSettings);
        Console.WriteLine($"Created Sink {logSink.Name}.");

        return logSink;
    }
}
// [END logging_create_sink]
