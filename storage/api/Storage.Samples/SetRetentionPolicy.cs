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

// [START storage_set_retention_policy]

using Google.Cloud.Storage.V1;
using System;
using static Google.Apis.Storage.v1.Data.Bucket;

public class SetRetentionPolicySample
{
    /// <summary>
    /// Sets the bucket's retention policy.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="retentionPeriod">The duration in seconds that objects need to be retained. The retention policy enforces a minimum retention
    /// time for all objects contained in the bucket, based on their creation time. Any
    /// attempt to overwrite or delete objects younger than the retention period will
    /// result in a PERMISSION_DENIED error. An unlocked retention policy can be modified
    /// or removed from the bucket via a storage.buckets.update operation. A locked retention
    /// policy cannot be removed or shortened in duration for the lifetime of the bucket.
    /// Attempting to remove or decrease the period of a locked retention policy will result
    /// in a PERMISSION_DENIED error.</param>
    public RetentionPolicyData SetRetentionPolicy(
        string bucketName = "your-unique-bucket-name",
        long retentionPeriod = 10)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        bucket.RetentionPolicy = new RetentionPolicyData { RetentionPeriod = retentionPeriod };

        bucket = storage.UpdateBucket(bucket);

        Console.WriteLine($"Retention policy for {bucketName} was set to {retentionPeriod}");
        return bucket.RetentionPolicy;
    }
}
// [END storage_set_retention_policy]
