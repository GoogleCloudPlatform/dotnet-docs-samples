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

// [START storage_set_bucket_public_iam]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class MakeBucketPublicSample
{
    public void MakeBucketPublic(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();

        Policy policy = storage.GetBucketIamPolicy(bucketName);

        policy.Bindings.Add(new Policy.BindingsData
        {
            Role = "roles/storage.objectViewer",
            Members = new List<string> { "allUsers" }
        });

        storage.SetBucketIamPolicy(bucketName, policy);
        Console.WriteLine(bucketName + " is now public ");
    }
}
// [END storage_set_bucket_public_iam]
