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

// [START storage_delete_bucket]

using Google.Cloud.Storage.V1;
using System;

public class DeleteBucketSample
{
    public void DeleteBucket(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        storage.DeleteBucket(bucketName);
        Console.WriteLine($"Deleted {bucketName}.");
    }
}
// [END storage_delete_bucket]
