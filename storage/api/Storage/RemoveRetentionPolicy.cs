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

// [START storage_remove_retention_policy]

using Google.Cloud.Storage.V1;
using System;

namespace Storage
{
    public class RemoveRetentionPolicy
    {
        public static void RemoveBucketRetentionPolicy(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            if (bucket.RetentionPolicy != null)
            {
                bool? isLockedOrNull = bucket?.RetentionPolicy.IsLocked;
                bool isLocked =
                    isLockedOrNull.HasValue ? isLockedOrNull.Value : false;
                if (isLocked)
                {
                    throw new Exception("Retention Policy is locked.");
                }

                bucket.RetentionPolicy.RetentionPeriod = null;
                bucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
                {
                    IfMetagenerationMatch = bucket.Metageneration
                });

                Console.WriteLine($"Retention period for {bucketName} has been removed.");
            }
        }
    }
}
// [END storage_remove_retention_policy]
