// Copyright 2021 Google Inc.
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

// [START storage_create_bucket_turbo_replication]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class CreateBucketWithTurboReplicationSample
{
    public Bucket CreateBucketWithTurboReplication(
        string projectId = "your-project-id",
        string location = "your-bucket-location",
        string bucketName = "bucket-name")
    {
        var storage = StorageClient.Create();
        // Enabling turbo replication requires a bucket with dual-region configuration
        var bucket = storage.CreateBucket(projectId, new Bucket { Name = bucketName , Location = location, Rpo = "ASYNC_TURBO" });

        Console.WriteLine($"Created {bucket.Name} in {bucket.Location} with RPO Setting {bucket.Rpo}.");
        return bucket;
    }

}

// [END storage_create_bucket_turbo_replication]
