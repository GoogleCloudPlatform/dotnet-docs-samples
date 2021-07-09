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

// [START storage_change_file_storage_class]

using Google.Cloud.Storage.V1;
using System;

public class ChangeFileStorageClassSample
{
    public Google.Apis.Storage.v1.Data.Object ChangeFileStorageClass(
        string bucketName = "your-bucket-name",
        string objectName = "your-object-name",
        string storageClass = StorageClasses.Standard)
    {
        var storage = StorageClient.Create();
        var file = storage.GetObject(bucketName, objectName);

        file.StorageClass = storageClass;

        file = storage.UpdateObject(file);
        Console.WriteLine($"Object {objectName} in bucket {bucketName} had" +
            $" its storage class set to {storageClass}.");
        return file;
    }
}
// [END storage_change_file_storage_class]
