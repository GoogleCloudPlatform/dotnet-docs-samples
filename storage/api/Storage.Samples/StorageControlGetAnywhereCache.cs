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

// [START storage_control_get_anywhere_cache]

using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlGetAnywhereCacheSample
{
    /// <summary>Gets an anywhere cache instance for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the anywhere cache instance.</param>
    /// <param name="anywhereCacheId">The unique identifier of the cache instance.</param>
    public AnywhereCache StorageControlGetAnywhereCache(string bucketName = "your-unique-bucket-name",
        string anywhereCacheId = "us-east1-b")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string anywhereCacheName = $"projects/_/buckets/{bucketName}/anywhereCaches/{anywhereCacheId}";

        var request = new GetAnywhereCacheRequest
        {
            Name = anywhereCacheName
        };

        AnywhereCache retrievedCache = storageControlClient.GetAnywhereCache(request);

        Console.WriteLine($"Got Anywhere Cache Instance: {retrievedCache.Name}");

        return retrievedCache;
    }
}
// [END storage_control_get_anywhere_cache]
