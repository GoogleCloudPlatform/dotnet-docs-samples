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

// [START storage_add_bucket_label]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class BucketAddLabelSample
{
    public Bucket BucketAddLabel(
        string bucketName = "your-bucket-name",
        string labelKey = "label-key-to-add",
        string labelValue = "label-value-to-add")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.Labels == null)
        {
            bucket.Labels = new Dictionary<string, string>();
        }
        bucket.Labels.Add(labelKey, labelValue);

        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"Added label {labelKey} with value {labelValue} to bucket {bucketName}.");
        return bucket;
    }
}
// [END storage_add_bucket_label]
