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

using Google.Cloud.Storage.V1;
using System;
using System.Diagnostics;

namespace GoogleCloudSamples
{
    class StorageQuickstart
    {
        static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            // [END storage_quickstart]
            Debug.Assert("YOUR-PROJECT-" + "ID" != projectId,
                "Edit Program.cs and replace YOUR-PROJECT-ID with your Google Project Id.");
            // [START storage_quickstart]

            // Instantiates a client.
            StorageClient storageClient = StorageClient.Create();

            // The name for the new bucket.
            string bucketName = projectId + "-test-bucket";
            try
            {
                // Creates the new bucket.
                storageClient.CreateBucket(projectId, bucketName);
                Console.WriteLine($"Bucket {bucketName} created.");
            }
            catch (Google.GoogleApiException e)
            when (e.Error.Code == 409)
            {
                // The bucket already exists.  That's fine.
                Console.WriteLine(e.Error.Message);
            }
        }
    }
}
// [END storage_quickstart]