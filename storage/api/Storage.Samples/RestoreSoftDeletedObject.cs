// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START storage_restore_object]

using Google.Cloud.Storage.V1;
using System;

public class RestoreSoftDeletedObjectSample
{
    /// <summary>
    /// Restores a soft deleted object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the soft-deleted object to restore.</param>
    /// <param name="generation">The generation number of the soft-deleted object to restore.</param>
    public Google.Apis.Storage.v1.Data.Object RestoreSoftDeletedObject(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        long generation = 1579287380533984)
    {
        var client = StorageClient.Create();
        var restoredObject = client.RestoreObject(bucketName, objectName, generation);
        Console.WriteLine($"The Name of the Restored Previously Soft-deleted Object is : {restoredObject.Name}");
        return restoredObject;
    }
}
// [END storage_restore_object]
