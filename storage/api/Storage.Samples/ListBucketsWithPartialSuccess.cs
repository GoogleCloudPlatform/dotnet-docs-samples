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

// [START storage_list_buckets_partial_success]

using Google.Api.Gax;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListBucketsWithPartialSuccessSample
{
    /// <summary>
    /// Lists buckets, returning both the reachable buckets and the resource names of buckets from unreachable locations when specific regions are unreachable.
    /// </summary>
    /// <param name="projectId">The ID of the project to list the buckets.</param>
    public (IReadOnlyList<Bucket> Reachable, IReadOnlyList<string> Unreachable) ListBucketsWithPartialSuccess
        (string projectId = "your-project-id")
    {
        var storage = StorageClient.Create();
        var pagedResult = storage.ListBuckets(projectId, options: new ListBucketsOptions
        {
            ReturnPartialSuccess = true
        });

        var reachableBuckets = new List<Bucket>();
        var unreachableBuckets = new List<string>();

        foreach (var page in pagedResult.AsRawResponses())
        {
            reachableBuckets.AddRange(page.Items ?? Enumerable.Empty<Bucket>());
            unreachableBuckets.AddRange(page.Unreachable ?? Enumerable.Empty<string>());
        }

        Console.WriteLine("Buckets:");
        foreach (var bucket in reachableBuckets)
        {
            Console.WriteLine(bucket.Name);
        }

        if (unreachableBuckets.Any())
        {
            Console.WriteLine("The Resource Names of Buckets from Unreachable Locations:");
            foreach (var bucket in unreachableBuckets)
            {
                Console.WriteLine(bucket);
            }
        }
        return (reachableBuckets, unreachableBuckets);
    }
}
// [END storage_list_buckets_partial_success]
