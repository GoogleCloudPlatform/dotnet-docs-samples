// Copyright 2021 Google Inc.
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

// [START storage_cors_configuration]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using static Google.Apis.Storage.v1.Data.Bucket;

public class BucketAddCorsConfigurationSample
{
    public Bucket BucketAddCorsConfiguration(string bucketName = "your-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        CorsData corsData = new CorsData
        {
            Origin = new string[] { "*" },
            ResponseHeader = new string[] { "Content-Type", "x-goog-resumable" },
            Method = new string[] { "PUT", "POST" },
            MaxAgeSeconds = 3600 //One Hour
        };

        if (bucket.Cors == null)
        {
            bucket.Cors = new List<CorsData>();
        }
        bucket.Cors.Add(corsData);

        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"bucketName {bucketName} was updated with a CORS config to allow {string.Join(",", corsData.Method)} requests from" +
            $" {string.Join(",", corsData.Origin)} sharing {string.Join(",", corsData.ResponseHeader)} responseHeader" +
            $" responses across origins.");
        return bucket;
    }
}
// [END storage_cors_configuration]
