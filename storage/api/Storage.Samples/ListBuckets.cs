// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_list_buckets]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class ListBucketsSample
{
    public IEnumerable<Bucket> ListBuckets(string projectId = "your-project-id")
    {
        var storage = StorageClient.Create();
        var buckets = storage.ListBuckets(projectId);
        Console.WriteLine("Buckets:");
        foreach (var bucket in buckets)
        {
            Console.WriteLine(bucket.Name);
        }
        return buckets;
    }
}
// [END storage_list_buckets]
