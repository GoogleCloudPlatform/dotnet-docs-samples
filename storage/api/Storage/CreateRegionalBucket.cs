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

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

namespace Storage
{
    public class CreateRegionalBucket
    {
        // [START storage_create_bucket_class_location]
        public static Bucket StorageCreateRegionalBucket(string projectId, string location, string bucketName)
        {
            var storage = StorageClient.Create();
            Bucket bucket = new Bucket { Location = location, Name = bucketName };
            var newlyCreatedBucket = storage.CreateBucket(projectId, bucket);
            Console.WriteLine($"Created {bucketName}.");
            return newlyCreatedBucket;
        }
        // [END storage_create_bucket_class_location]
    }
}
