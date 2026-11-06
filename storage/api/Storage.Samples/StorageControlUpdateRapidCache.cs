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

// [START storage_control_update_rapid_cache]

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

public class StorageControlUpdateRapidCacheSample
{
    /// <summary>Updates the time to live (TTL) of a Rapid Cache instance.</summary>
    /// <param name="bucketName">The name of the bucket that owns the Rapid Cache instance.</param>
    /// <param name="rapidCacheId">The unique identifier of the Rapid Cache instance.</param>
    /// <param name="ttlSeconds">The new TTL for cache entries, in seconds. The TTL must be between 1 hour and 7 days.</param>
    public RapidCache StorageControlUpdateRapidCache(string bucketName = "your-unique-bucket-name",
        string rapidCacheId = "your-rapid-cache-id",
        long ttlSeconds = 172800)
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", bucketName, rapidCacheId);

        RapidCache rapidCache = new RapidCache
        {
            Name = rapidCacheName,
            Ttl = new Duration { Seconds = ttlSeconds }
        };

        // Only the fields listed in the update mask are changed.
        FieldMask updateMask = new FieldMask { Paths = { "ttl" } };

        UpdateRapidCacheRequest request = new UpdateRapidCacheRequest
        {
            RapidCache = rapidCache,
            UpdateMask = updateMask
        };

        // Updating a Rapid Cache is a long-running operation (LRO).
        Operation<RapidCache, UpdateRapidCacheMetadata> operation = storageControlClient.UpdateRapidCache(request);

        // Await the LRO's completion.
        RapidCache updatedCache = operation.PollUntilCompleted().Result;

        Console.WriteLine($"Updated Rapid Cache: {updatedCache.Name}");
        Console.WriteLine($"TTL: {updatedCache.Ttl.Seconds} seconds");

        return updatedCache;
    }
}
// [END storage_control_update_rapid_cache]
