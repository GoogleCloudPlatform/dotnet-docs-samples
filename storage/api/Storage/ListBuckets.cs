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

using Google.Cloud.Storage.V1;
using System;

namespace Storage
{
    public class ListBuckets
    {
        // [START storage_list_buckets]
        public static void GetBucketList(string projectId)
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListBuckets(projectId))
            {
                Console.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_buckets]
    }
}
