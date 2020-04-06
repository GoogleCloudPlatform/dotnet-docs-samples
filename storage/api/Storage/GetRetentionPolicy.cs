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

// [START storage_get_retention_policy]
using Google.Cloud.Storage.V1;
using System;
using static Google.Apis.Storage.v1.Data.Bucket;

namespace Storage
{
    public class GetRetentionPolicy
    {
        public static RetentionPolicyData GetBucketRetentionPolicy(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);

            if (bucket.RetentionPolicy != null)
            {
                Console.WriteLine("Retention policy:");
                Console.WriteLine($"period: {bucket.RetentionPolicy.RetentionPeriod}");
                Console.WriteLine($"effective time: {bucket.RetentionPolicy.EffectiveTime}");
                bool? isLockedOrNull = bucket?.RetentionPolicy.IsLocked;
                bool isLocked =
                    isLockedOrNull.HasValue ? isLockedOrNull.Value : false;
                Console.WriteLine("policy locked: {0}", isLocked);
            }
            return bucket.RetentionPolicy;
        }
    }
}
// [END storage_get_retention_policy]
