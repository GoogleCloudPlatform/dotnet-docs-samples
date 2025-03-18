// Copyright 2024 Google Inc.
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

// [START storage_get_soft_deleted_bucket]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class GetSoftDeletedBucketSample
{
    /// <summary>
    /// Get a soft deleted bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="generation">The generation of the bucket.</param>
    public Bucket GetSoftDeletedBucket(
        string bucketName = "your-unique-bucket-name",
        long? generation = 123456789)
    {
        var client = StorageClient.Create();
        var bucket = client.GetBucket(bucketName, new GetBucketOptions { SoftDeleted = true, Generation = generation });
        Console.WriteLine($"Bucket:\t{bucket.Name}");
        Console.WriteLine($"Bucket Generation:\t{bucket.Generation}");
        Console.WriteLine($"Bucket SoftDelete Time:\t{bucket.SoftDeleteTimeDateTimeOffset}");
        Console.WriteLine($"Bucket HardDelete Time:\t{bucket.HardDeleteTimeDateTimeOffset}");
        return bucket;
    }
}
// [END storage_get_soft_deleted_bucket]
