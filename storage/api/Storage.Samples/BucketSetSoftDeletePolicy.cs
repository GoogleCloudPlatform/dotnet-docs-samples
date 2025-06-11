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

// [START storage_set_soft_delete_policy]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketSetSoftDeletePolicySample
{
    /// <summary>
    /// Set soft delete policy for the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="retentionDurationInDays">The retention duration in days to set soft-delete policy for the bucket.</param>
    public Bucket BucketSetSoftDeletePolicy(string bucketName = "your-unique-bucket-name",
        int retentionDurationInDays = 10)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDays).TotalSeconds;
        if (retentionDurationInDays < 7 || retentionDurationInDays > 90)
        {
            // The maximum retention duration you can set is 90 days and the minimum retention duration you can set is 7 days.
            // For more information, please refer to https://cloud.google.com/storage/docs/soft-delete#retention-duration
            Console.WriteLine($"The Soft Delete Policy for the Bucket (Bucket Name: {bucketName}) must have a retention duration between 7 days and 90 days");
            return bucket;
        }
        bucket.SoftDeletePolicy = new Bucket.SoftDeletePolicyData { RetentionDurationSeconds = retentionDurationInSeconds };
        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"The Soft Delete Policy for the Bucket (Bucket Name: {bucketName}) is set to the {retentionDurationInDays} days retention duration");
        return bucket;
    }
}
// [END storage_set_soft_delete_policy]
