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

// [START storage_control_update_anywhere_cache]

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

public class StorageControlUpdateAnywhereCacheSample
{
    /// <summary>Updates the running anywhere cache instance for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the anywhere cache instance.</param>
    /// <param name="anywhereCacheId">The name of the zone in which the anywhere cache is located.</param>
    /// <param name="admissionPolicy"> The cache's admission policy. Values can be admit-on-first-miss or admit-on-second-miss. If not specified, it defaults to admit-on-first-miss.</param>
    public Operation<AnywhereCache, UpdateAnywhereCacheMetadata> StorageControlUpdateAnywhereCache(string bucketName = "your-bucket-name",
        string anywhereCacheId = "us-east1-a",
        string admissionPolicy = "admit-on-first-miss")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string anywhereCacheName = $"projects/_/buckets/{bucketName}/anywhereCaches/{anywhereCacheId}";

        var anywhereCache = new AnywhereCache
        {
            Name = anywhereCacheName,
            AdmissionPolicy = admissionPolicy
        };
        FieldMask fieldMask = new FieldMask { Paths = { "admission_policy" } };

        var request = new UpdateAnywhereCacheRequest
        {
            AnywhereCache = anywhereCache,
            UpdateMask = fieldMask
        };

        // Start a long-running operation (LRO).
        Operation<AnywhereCache, UpdateAnywhereCacheMetadata> updatedCacheOperation = storageControlClient.UpdateAnywhereCache(request);

        // Await the LROs completion.
        var updatedCache = updatedCacheOperation.PollUntilCompleted();

        Console.WriteLine($"Updated Anywhere Cache Instance: {updatedCache.Result.Name}, New Cache Admission Policy: {updatedCache.Result.AdmissionPolicy}");

        return updatedCache;
    }
}
// [END storage_control_update_anywhere_cache]
