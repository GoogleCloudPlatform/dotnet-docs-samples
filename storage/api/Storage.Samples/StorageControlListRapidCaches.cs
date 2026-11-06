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

// [START storage_control_list_rapid_caches]

using Google.Api.Gax;
using Google.Cloud.Storage.Control.V2;
using System;
using System.Collections.Generic;

public class StorageControlListRapidCachesSample
{
    /// <summary>Lists all Rapid Cache instances for the specified bucket.</summary>
    /// <param name="bucketName">The name of the bucket that owns the Rapid Cache instances.</param>
    public IEnumerable<RapidCache> StorageControlListRapidCaches(string bucketName = "your-unique-bucket-name")
    {
        StorageControlClient storageControlClient = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket.
        string parent = $"projects/_/buckets/{bucketName}";

        ListRapidCachesRequest request = new ListRapidCachesRequest
        {
            Parent = parent
        };

        // The returned sequence fetches additional pages automatically as it is iterated.
        PagedEnumerable<ListRapidCachesResponse, RapidCache> rapidCaches = storageControlClient.ListRapidCaches(request);

        List<RapidCache> results = new List<RapidCache>();
        foreach (RapidCache rapidCache in rapidCaches)
        {
            Console.WriteLine(rapidCache.Name);
            results.Add(rapidCache);
        }

        Console.WriteLine($"Listed Rapid Caches in bucket {bucketName}");

        return results;
    }
}
// [END storage_control_list_rapid_caches]
