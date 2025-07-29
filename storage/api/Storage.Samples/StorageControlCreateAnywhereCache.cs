// Copyright 2025 Google LLC
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

// [START storage_control_create_anywhere_cache]

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using System;
using System.Threading.Tasks;

public class StorageControlCreateAnywhereCacheSample
{
    /// <summary>Creates an anywhere cache instance in the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="zone">The zone in which the cache instance will run.</param>
    public async Task<AnywhereCache> StorageControlCreateAnywhereCacheAsync(string bucketName = "your-bucket-name",
        string zone = "us-east-a")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string parent = $"projects/_/buckets/{bucketName}";

        AnywhereCache anywhereCache = new AnywhereCache
        {
            Zone = zone
        };

        var request = new CreateAnywhereCacheRequest
        {
            AnywhereCache = anywhereCache,
            Parent = parent
        };

        // Start a long-running operation (LRO).
        Task<Operation<AnywhereCache, CreateAnywhereCacheMetadata>> createdCacheOperation = storageControlClient.CreateAnywhereCacheAsync(request);

        // Await the LROs completion.
        var createdCache = await createdCacheOperation.Result.PollUntilCompletedAsync();

        Console.WriteLine($"Created Anywhere Cache Instance: {createdCache.Result.AnywhereCacheName}");

        return createdCache.Result;
    }
}
// [END storage_control_create_anywhere_cache]
