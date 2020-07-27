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

// [START storage_lock_retention_policy]

using Google.Cloud.Storage.V1;
using System;

public class LockRetentionPolicySample
{
    /// <summary>
    /// Locks the retention policy of a bucket. This is a one-way process: once a retention
    /// policy is locked, it cannot be shortened, removed or unlocked, although it can
    /// be increased in duration. The lock persists until the bucket is deleted.
    /// </summary>
    /// <param name="bucketName">The name of the bucket whose retention policy should be locked.</param>
    public bool? LockRetentionPolicy(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        storage.LockBucketRetentionPolicy(bucketName, bucket.Metageneration.Value);
        bucket = storage.GetBucket(bucketName);
        Console.WriteLine($"Retention policy for {bucketName} is now locked");
        Console.WriteLine($"Retention policy effective as of {bucket.RetentionPolicy.EffectiveTime}");

        return bucket.RetentionPolicy.IsLocked;
    }
}
// [END storage_lock_retention_policy]
