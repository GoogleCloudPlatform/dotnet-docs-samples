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

using Google.Api.Gax;
using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using System;

public class StorageControlDisableRapidCacheSample
{
    /// <summary>Disables a Rapid Cache instance for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the Rapid Cache instance.</param>
    /// <param name="rapidCacheId">The unique identifier of the Rapid Cache instance.</param>
    /// <param name="timeout">Optional maximum time to wait for the operation to complete.
    /// If null, the client's default polling settings are used.</param>
    public RapidCache StorageControlDisableRapidCache(
        string bucketName = "your-unique-bucket-name",
        string rapidCacheId = "your-rapid-cache-id",
        TimeSpan? timeout = null)
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", bucketName, rapidCacheId);

        DisableRapidCacheRequest request = new DisableRapidCacheRequest
        {
            Name = rapidCacheName
        };

        // Disabling a Rapid Cache is a long-running operation (LRO).
        Operation<RapidCache, DisableRapidCacheMetadata> operation = storageControlClient.DisableRapidCache(request);

        // Wait for the long-running operation to complete. A TimeoutException is thrown if
        // the operation doesn't complete within the optional timeout.
        PollSettings pollSettings = timeout.HasValue
            ? new PollSettings(Expiration.FromTimeout(timeout.Value), TimeSpan.FromSeconds(10))
            : null;
        RapidCache disabledCache = operation.PollUntilCompleted(pollSettings).Result;

        Console.WriteLine($"Disabled Rapid Cache: {disabledCache.Name}");
        Console.WriteLine($"State: {disabledCache.State}");

        return disabledCache;
    }
}
// [END storage_control_disable_rapid_cache]
