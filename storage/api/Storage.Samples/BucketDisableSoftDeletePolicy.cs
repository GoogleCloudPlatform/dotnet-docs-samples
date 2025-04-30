// Copyright 2025 Google Inc.
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
    /// <param name="retentionDurationInDay">The retention duration to disable soft-delete policy for the bucket.</param>
    public Bucket BucketDisableSoftDeletePolicy(string bucketName = "your-unique-bucket-name",
        int retentionDurationInDay = 0)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDay).TotalSeconds;
        bucket.SoftDeletePolicy = new Bucket.SoftDeletePolicyData { RetentionDurationSeconds = retentionDurationInSeconds };
        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"Soft Delete Policy for the {bucketName} is disabled");
        return bucket;
    }
}
// [END storage_disable_soft_delete]
