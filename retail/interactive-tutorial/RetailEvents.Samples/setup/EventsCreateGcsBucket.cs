// Copyright 2021 Google Inc. All Rights Reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;

public static class EventsCreateGcsBucket
{
    private const string EventsFileName = "user_events.json";
    private const string InvalidEventsFileName = "user_events_some_invalid.json";

    private static string requestTimeStamp = DateTime.Now.ToUniversalTime().ToString("ddMMyyyyhhmmss");

    private static readonly string eventsFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailEvents.Samples/resources/{EventsFileName}");
    private static readonly string invalidEventsFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailEvents.Samples/resources/{InvalidEventsFileName}");
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string bucketName = $"{projectId}_events_{requestTimeStamp}";

    private static readonly StorageClient storageClient = StorageClient.Create();

    /// <summary>
    /// Get the current solution directory full name.
    /// </summary>
    /// <param name="currentPath">The current path.</param>
    /// <returns>Full name of the current solution directory.</returns>
    private static string GetSolutionDirectoryFullName(string currentPath = null)
    {
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory.FullName;
    }

    /// <summary>Create GCS bucket.</summary>
    private static Bucket CreateBucket(string bucketName)
    {
        var newBucket = new Bucket();
        Console.WriteLine($"\nBucket name: {bucketName}\n");

        var bucketExists = CheckIfBucketExists(bucketName);

        if (bucketExists)
        {
            Console.WriteLine($"\nBucket {bucketName} already exists.\n");
            return storageClient.GetBucket(bucketName);
        }
        else
        {
            var bucket = new Bucket
            {
                Name = bucketName,
                StorageClass = "STANDARD",
                Location = "us"
            };

            newBucket = storageClient.CreateBucket(projectId, bucket);

            Console.WriteLine($"\nCreated bucket {newBucket.Name} in {newBucket.Location} with storage class {newBucket.StorageClass}\n");
        };

        return newBucket;
    }

    private static bool CheckIfBucketExists(string newBucketName)
    {
        var bucketExists = false;
        var bucketsInYourProject = ListBuckets();
        var bucketNamesInYourProject = bucketsInYourProject.Select(x => x.Name).ToArray();

        foreach (var existingBucketName in bucketNamesInYourProject)
        {
            if (existingBucketName == newBucketName)
            {
                bucketExists = true;
                break;
            }
        }

        return bucketExists;
    }

    /// <summary>List all existing buckets.</summary>
    private static List<Bucket> ListBuckets()
    {
        var bucketsList = new List<Bucket>();
        var buckets = storageClient.ListBuckets(projectId);

        foreach (var bucket in buckets)
        {
            bucketsList.Add(bucket);
            Console.WriteLine(bucket.Name);
        }

        return bucketsList;
    }

    /// <summary>Upload blob.</summary>
    private static void UploadBlob(string bucketName, string localPath, string objectName)
    {
        var bucket = storageClient.GetBucket(bucketName);

        using var fileStream = File.OpenRead(localPath);
        storageClient.UploadObject(bucketName, objectName, null, fileStream);
        Console.WriteLine($"Uploaded {objectName}.");
    }

    /// <summary>
    /// Create test resources.
    /// </summary>
    /// <returns>The name of created bucket.</returns>
    public static string PerformCreationOfEventsGcsBucket()
    {
        // Create a GCS bucket.
        Bucket createdProductsBucket = CreateBucket(bucketName);

        // Upload user_events.json file to a bucket.
        UploadBlob(createdProductsBucket.Name, eventsFilePath, EventsFileName);

        // Upload user_events_some_invalid.json file to a bucket.
        // UploadBlob(createdProductsBucket.Name, invalidEventsFilePath, InvalidEventsFileName);

        return createdProductsBucket.Name;
    }
}