// Copyright 2025 Google Inc.
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

public class ListSoftDeletedVersionOfObjectSample
{
    /// <summary>
    /// List soft deleted versions of the object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the soft-deleted object.</param>
    public IEnumerable<Google.Apis.Storage.v1.Data.Object> ListSoftDeletedVersionOfObject(string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var objects = storage.ListObjects(bucketName, null, new ListObjectsOptions { SoftDeletedOnly = true, MatchGlob = objectName });
        Console.WriteLine("Soft Deleted Versions of the Object with the Name and Versions are as follows:");
        foreach (var obj in objects)
        {
            Console.WriteLine($"Name: {obj.Name} and Version : {obj.Generation}");
        }
        return objects;
    }
}
// [END storage_list_soft_deleted_object_versions]
