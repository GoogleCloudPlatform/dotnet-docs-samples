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

// [START storage_get_soft_delete_policy]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketGetSoftDeletePolicySample
{
    /// <summary>
    /// Get soft delete policy of the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    public Bucket BucketGetSoftDeletePolicy(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        if (bucket.SoftDeletePolicy.RetentionDurationSeconds == 0)
        {
            Console.WriteLine($"The Soft Delete Policy is disabled for the {bucketName}");
        }
        else
        {
            int retentionDuration = (int) (bucket.SoftDeletePolicy.RetentionDurationSeconds / (24 * 60 * 60));
            Console.WriteLine($"The Soft Delete Policy for the {bucketName} is : {retentionDuration} days");
        }
        return bucket;
    }
}
// [END storage_get_soft_delete_policy]
