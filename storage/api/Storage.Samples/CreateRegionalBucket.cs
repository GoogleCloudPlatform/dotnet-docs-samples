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

// [START storage_create_bucket_class_location]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreateRegionalBucketSample
{
    /// <summary>
    /// Creates a storage bucket with region.
    /// </summary>
    /// <param name="projectId">The ID of the project to create the buckets in.</param>
    /// <param name="location">The location of the bucket. Object data for objects in the bucket resides in 
    /// physical storage within this region. Defaults to US.</param>
    /// <param name="bucketName">The name of the bucket to create.</param>
    /// <param name="storageClass">The bucket's default storage class, used whenever no storageClass is specified
    /// for a newly-created object. This defines how objects in the bucket are stored
    /// and determines the SLA and the cost of storage. Values include MULTI_REGIONAL,
    /// REGIONAL, STANDARD, NEARLINE, COLDLINE, ARCHIVE, and DURABLE_REDUCED_AVAILABILITY.
    /// If this value is not specified when the bucket is created, it will default to
    /// STANDARD.</param>
    public Bucket CreateRegionalBucket(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name",
        string location = "us-west1",
        string storageClass = "REGIONAL")
    {
        var storage = StorageClient.Create();
        Bucket bucket = new Bucket
        {
            Location = location,
            Name = bucketName,
            StorageClass = storageClass
        };
        var newlyCreatedBucket = storage.CreateBucket(projectId, bucket);
        Console.WriteLine($"Created {bucketName}.");
        return newlyCreatedBucket;
    }
}
// [END storage_create_bucket_class_location]
