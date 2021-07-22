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

// [START storage_get_public_access_prevention]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class GetPublicAccessPreventionSample
{
    public string GetPublicAccessPrevention(string bucketName = "your-unique-bucket-name")
    {
        // Gets Bucket Metadata and prints publicAccessPrevention value (either "unspecified" or "enforced").
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        var publicAccessPrevention = bucket.IamConfiguration.PublicAccessPrevention;

        Console.WriteLine($"Public access prevention is {publicAccessPrevention} for {bucketName}.");
        return publicAccessPrevention;
    }
}
// [END storage_get_public_access_prevention]
