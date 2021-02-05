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

// [START storage_set_metadata]

using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class SetObjectMetadataSample
{
    public Google.Apis.Storage.v1.Data.Object SetObjectMetadata(
        string bucketName = "your-bucket-name",
        string objectName = "your-object-name",
        string key = "key-to-add",
        string value = "value-to-add")
    {
        var storage = StorageClient.Create();
        var file = storage.GetObject(bucketName, objectName);

        if (file.Metadata == null)
        {
            file.Metadata = new Dictionary<string, string>();
        }
        file.Metadata.Add(key, value);

        file = storage.UpdateObject(file);
        Console.WriteLine($"Updated custom metadata for object {objectName} in bucket {bucketName}");
        return file;
    }
}
// [END storage_set_metadata]
