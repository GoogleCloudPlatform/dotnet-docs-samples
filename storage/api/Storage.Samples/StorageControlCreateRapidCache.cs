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

// [START storage_control_create_rapid_cache]

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using System;

public class StorageControlCreateRapidCacheSample
{
    /// <summary>
    /// Creates a Rapid Cache instance for the specified bucket. Only the required fields are
    /// set; optional settings such as the time to live (TTL) use the server defaults and can be
    /// changed later with an update.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="zone">The zone in which to create the Rapid Cache. The zone must be in the same region as the bucket.</param>
    public RapidCache StorageControlCreateRapidCache(string bucketName = "your-unique-bucket-name",
        string zone = "us-central1-a")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string parent = $"projects/_/buckets/{bucketName}";

        // The Rapid Cache ID must be the same as the zone.
        string rapidCacheId = zone;
        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", bucketName, rapidCacheId);

        // Rapid Cache Ultra requires a regional bucket with hierarchical namespace enabled,
        // in the same region as the cache zone.
        RapidCache rapidCache = new RapidCache
        {
            Name = rapidCacheName,
            Zone = zone,
            CacheType = "rapid-cache-ultra"
        };

        CreateRapidCacheRequest request = new CreateRapidCacheRequest
        {
            Parent = parent,
            RapidCache = rapidCache
        };

        // Creating a Rapid Cache is a long-running operation (LRO).
        Operation<RapidCache, CreateRapidCacheMetadata> operation = storageControlClient.CreateRapidCache(request);

        // Await the LRO's completion.
        RapidCache createdCache = operation.PollUntilCompleted().Result;

        Console.WriteLine($"Created Rapid Cache: {createdCache.Name}");
        Console.WriteLine($"State: {createdCache.State}");

        return createdCache;
    }
}
// [END storage_control_create_rapid_cache]
