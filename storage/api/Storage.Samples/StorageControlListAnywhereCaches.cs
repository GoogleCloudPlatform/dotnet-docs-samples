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

// [START storage_control_list_anywhere_caches]

using Google.Api.Gax;
using Google.Cloud.Storage.Control.V2;
using System;
using System.Collections.Generic;

public class StorageControlListAnywhereCachesSample
{
    /// <summary>Lists all anywhere cache instances for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the anywhere cache instance.</param>
    /// <param name="pageSize">The maximum number of anywhere cache instances to return in a single response.</param>
    public IEnumerable<AnywhereCache> StorageControlListAnywhereCaches(string bucketName = "your-unique-bucket-name", int pageSize = 10)
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string parent = $"projects/_/buckets/{bucketName}";

        var request = new ListAnywhereCachesRequest
        {
            Parent = parent,
            PageSize = pageSize
        };

        PagedEnumerable<ListAnywhereCachesResponse, AnywhereCache> anywhereCaches = storageControlClient.ListAnywhereCaches(request);

        Console.WriteLine($"The Names of Anywhere Cache Instances are as follows:");

        foreach (AnywhereCache cache in anywhereCaches)
        {
            Console.WriteLine($"Anywhere Cache Instance: {cache.Name}");
        }

        // Retrieve a single page of page size (unless it's the final page).
        Page<AnywhereCache> singlePage = anywhereCaches.ReadPage(pageSize);

        Console.WriteLine($"The Names of Anywhere Cache Instances in the Page of Page Size {pageSize} are as follows:");

        foreach (AnywhereCache cache in singlePage)
        {
            Console.WriteLine($"Anywhere Cache Instance: {cache.Name}");
        }

        return anywhereCaches;
    }
}
// [END storage_control_list_anywhere_caches]
