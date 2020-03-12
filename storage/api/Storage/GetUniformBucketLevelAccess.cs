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

using Google.Cloud.Storage.V1;
using System;

namespace Storage
{
    public class GetUniformBucketLevelAccess
    {
        // [START storage_get_uniform_bucket_level_access]
        public static void StorageGetUniformBucketLevelAccess(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            var uniformBucketLevelAccess = bucket.IamConfiguration.UniformBucketLevelAccess;

            bool? enabledOrNull = uniformBucketLevelAccess?.Enabled;
            bool uniformBucketLevelAccessEnabled =
                enabledOrNull.HasValue ? enabledOrNull.Value : false;
            if (uniformBucketLevelAccessEnabled)
            {
                Console.WriteLine($"Uniform bucket-level access is enabled for {bucketName}.");
                Console.WriteLine(
                    $"Uniform bucket-level access will be locked on {uniformBucketLevelAccess.LockedTime}.");
            }
            else
            {
                Console.WriteLine($"Uniform bucket-level access is not enabled for {bucketName}.");
            }
        }
        // [END storage_get_uniform_bucket_level_access]
    }
}
