// Copyright 2025 Google LLC
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

// [START storage_list_soft_deleted_object_versions]

using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class ListSoftDeletedVersionsOfObjectSample
{
    /// <summary>
    /// List all soft-deleted versions of the object in the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    public IEnumerable<Google.Apis.Storage.v1.Data.Object> ListSoftDeletedVersionsOfObject(string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var objects = storage.ListObjects(bucketName, prefix: null, new ListObjectsOptions { SoftDeletedOnly = true, MatchGlob = objectName });
        Console.WriteLine($"The Name and Versions of the Soft Deleted Object in the Bucket (Bucket Name: {bucketName}) are as follows:");
        foreach (var obj in objects)
        {
            Console.WriteLine($"Object Name: {obj.Name} and Version: {obj.Generation}");
        }
        return objects;
    }
}
// [END storage_list_soft_deleted_object_versions]
