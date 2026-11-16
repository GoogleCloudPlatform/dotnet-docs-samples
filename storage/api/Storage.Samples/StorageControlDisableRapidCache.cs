// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_control_disable_rapid_cache]

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using System;

public class StorageControlDisableRapidCacheSample
{
    /// <summary>Disables a Rapid Cache instance for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the Rapid Cache instance.</param>
    /// <param name="rapidCacheId">The unique identifier of the Rapid Cache instance, which is the
    /// zone of the cache (for example, "us-central1-a").</param>
    public Operation<RapidCache, DisableRapidCacheMetadata> StorageControlDisableRapidCache(
        string bucketName = "your-unique-bucket-name",
        string rapidCacheId = "us-central1-a")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", bucketName, rapidCacheId);

        DisableRapidCacheRequest request = new DisableRapidCacheRequest
        {
            Name = rapidCacheName
        };

        // Disabling a Rapid Cache is a long-running operation (LRO). The cache moves to the
        // "disabled" state within seconds, but the operation only completes once the cache has
        // been fully removed, which can take more than an hour. Instead of waiting for the
        // operation, callers can use StorageControlGetRapidCache to check for the "disabled" state.
        Operation<RapidCache, DisableRapidCacheMetadata> operation = storageControlClient.DisableRapidCache(request);

        Console.WriteLine($"Started disabling Rapid Cache {rapidCacheName}. Operation: {operation.Name}");

        return operation;
    }
}
// [END storage_control_disable_rapid_cache]
