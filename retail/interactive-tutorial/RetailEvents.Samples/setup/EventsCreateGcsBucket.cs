// Copyright 2022 Google Inc.

//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Google.Apis.Storage.v1.Data;

public static class EventsCreateGcsBucket
{
    private const string EventsFileName = "user_events.json";
    private const string InvalidEventsFileName = "user_events_some_invalid.json";

    private static readonly string requestTimeStamp = DateTime.UtcNow.ToString("ddMMyyyyhhmmss");

    private static readonly string eventsFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{EventsFileName}");
    private static readonly string invalidEventsFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{InvalidEventsFileName}");
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string bucketName = $"{projectId}_events_{requestTimeStamp}";

    /// <summary>
    /// Create events GCS bucket with data.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformCreationOfEventsGcsBucket()
    {
        // Create a GCS bucket.
        Bucket createdProductsBucket = CreateTestResources.CreateBucket(bucketName);

        // Upload user_events.json file to a bucket.
        CreateTestResources.UploadBlob(createdProductsBucket.Name, eventsFilePath, EventsFileName);

        // Upload user_events_some_invalid.json file to a bucket.
        CreateTestResources.UploadBlob(createdProductsBucket.Name, invalidEventsFilePath, InvalidEventsFileName);
    }
}
