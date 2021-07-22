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

// [START storage_set_public_access_prevention_unspecified]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class SetPublicAccessPreventionUnspecifiedSample
{
    public Bucket SetPublicAccessPreventionUnspecified(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        // Sets public access prevention to "unspecified" for the bucket.
        bucket.IamConfiguration.PublicAccessPrevention = "unspecified";
        bucket = storage.UpdateBucket(bucket);

        Console.WriteLine($"Public access prevention is 'unspecified' for {bucketName}.");
        return bucket;
    }
}
// [END storage_set_public_access_prevention_unspecified]
