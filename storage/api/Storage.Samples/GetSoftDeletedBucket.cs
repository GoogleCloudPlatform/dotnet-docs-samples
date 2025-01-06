// Copyright 2024 Google Inc.
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

// [START storage_get_soft_deleted_bucket]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Threading.Tasks;

public class GetSoftDeletedBucketSample
{
    public  Bucket GetSoftDeletedBucket(string bucketName = "your-unique-bucket-name")
    {
        var client = StorageClient.Create(); 
        var bucket = client.GetBucket(bucketName, new GetBucketOptions { SoftDeleted = false });
        client.DeleteBucket(bucketName, new DeleteBucketOptions { DeleteObjects = true });
        var softDeleted =  client.GetBucket(bucketName, new GetBucketOptions { SoftDeleted = true, Generation = bucket.Generation });
        Console.WriteLine($"Bucket:\t{softDeleted.Name}");
        Console.WriteLine($"Bucket Generation:\t{softDeleted.Generation}");
        Console.WriteLine($"Bucket SoftDelete Time:\t{softDeleted.SoftDeleteTimeDateTimeOffset}");
        Console.WriteLine($"Bucket HardDelete Time:\t{softDeleted.HardDeleteTimeDateTimeOffset}");
        return softDeleted;
    }
}
// [END storage_get_soft_deleted_bucket]
