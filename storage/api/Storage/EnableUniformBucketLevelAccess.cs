﻿// Copyright 2020 Google Inc.
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
    public class EnableUniformBucketLevelAccess
    {
        // [START storage_enable_uniform_bucket_level_access]
        public static void StorageEnableUniformBucketLevelAccess(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            bucket.IamConfiguration.UniformBucketLevelAccess.Enabled = true;
            bucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Use IfMetagenerationMatch to avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });

            Console.WriteLine($"Uniform bucket-level access was enabled for {bucketName}.");
        }
        // [END storage_enable_uniform_bucket_level_access]
    }
}
