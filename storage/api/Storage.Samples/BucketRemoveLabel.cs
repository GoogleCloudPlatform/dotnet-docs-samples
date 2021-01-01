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

// [START storage_remove_bucket_label]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketRemoveLabelSample
{
    public Bucket BucketRemoveLabel(string bucketName = "your-bucket-name", string labelKey = "label-key-to-remove")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.Labels != null && bucket.Labels.Keys.Contains(labelKey))
        {
            bucket.Labels.Remove(labelKey);
            bucket = storage.UpdateBucket(bucket);
            Console.WriteLine($"Removed label {labelKey} from bucket {bucketName}.");
        }
        else
        {
            Console.WriteLine($"No such label available on {bucketName}.");
        }
        return bucket;
    }
}
// [END storage_remove_bucket_label]
