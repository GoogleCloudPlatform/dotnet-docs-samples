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

// [START storage_set_bucket_public_iam]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class MakeBucketPublicSample
{
    public string MakeBucketPublic(
        string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName); 

        Policy originalPolicy = storage.GetBucketIamPolicy(bucketName);
        var binding = originalPolicy.Bindings;

        foreach(var bind in binding)
        {
            if (bind.Role == "roles/storage.legacyBucketReader")
            {
                bind.Members = new List<String>() { "allUsers" };
                break;
            }
        }
        
        originalPolicy.Bindings = binding;
        storage.SetBucketIamPolicy(bucketName, originalPolicy);

        Console.WriteLine(bucketName + " is now public ");

        return bucket.Name;
    }
}
// [END  storage_set_bucket_public_iam]
