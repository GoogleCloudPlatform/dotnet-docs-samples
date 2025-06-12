// Copyright 2025 Google LLC
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

// [START storage_disable_soft_delete]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketDisableSoftDeletePolicySample
{
    /// <summary>
    /// Disable soft delete policy for the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    public Bucket BucketDisableSoftDeletePolicy(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        // To disable soft-delete policy for the bucket, set the soft delete retention duration to 0 seconds.
        bucket.SoftDeletePolicy = new Bucket.SoftDeletePolicyData { RetentionDurationSeconds = 0L };
        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"The Soft Delete Policy for the Bucket (Bucket Name: {bucketName}) is disabled");
        return bucket;
    }
}
// [END storage_disable_soft_delete]
