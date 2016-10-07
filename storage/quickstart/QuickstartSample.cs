// Copyright 2016 Google Inc.
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

// [START storage_quickstart]
using System;
// Imports the Google Cloud client library
using Google.Storage.V1;

public class QuickstartSample
{
    public static void Main()
    {
        // Your Google Cloud Platform project ID
        string projectId = "YOUR_PROJECT_ID";

        // Instantiates a client
        StorageClient storageClient = StorageClient.Create();

        // The name for the new bucket
        string bucketName = "my-new-bucket";

        // Creates the new bucket
        storageClient.CreateBucket(projectId, bucketName);

        Console.WriteLine($"Bucket {bucket.Name} created.");
    }
}
// [END storage_quickstart]
