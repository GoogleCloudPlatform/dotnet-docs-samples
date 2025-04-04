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

// [START storage_restore_bucket]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
public class RestoreSoftDeletedBucketSample
{
    /// <summary>
    /// Restores a soft deleted bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket to restore.</param>
    /// <param name="generation">The generation of the bucket.</param>
    public Bucket RestoreSoftDeletedBucket(
        string bucketName = "your-unique-bucket-name",
        long generation = 123456789)
    {
        var client = StorageClient.Create();
        var restored = client.RestoreBucket(bucketName, generation);
        Console.WriteLine($"Bucket Name:\t {restored.Name}");
        Console.WriteLine($"Bucket Generation:\t {restored.Generation}");
        return restored;
    }
}
// [END storage_restore_bucket]
