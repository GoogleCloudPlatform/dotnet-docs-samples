// Copyright 2026 Google LLC
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

// [START storage_delete_object_contexts]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class DeleteObjectContextsSample
{
    /// <summary>
    /// Deletes all the custom context data associated with the object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket containing the object.</param>
    /// <param name="objectName">The name of the object.</param>
    public Google.Apis.Storage.v1.Data.Object DeleteObjectContexts(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name")
    {
        var storage = StorageClient.Create();
        var obj = storage.GetObject(bucketName, objectName);

        // To delete all contexts, set an empty custom dictionary. Note that UpdateObject
        // performs a full replacement of the object's writable metadata, so the explicit
        // empty dictionary makes the intent to clear every custom context unambiguous.
        obj.Contexts = new Google.Apis.Storage.v1.Data.Object.ContextsData
        {
            Custom = new Dictionary<string, ObjectCustomContextPayload>()
        };

        // Optional: set a metageneration-match precondition to avoid potential race conditions
        // and data corruptions. The request to update is aborted if the object's
        // metageneration does not match your precondition.
        var options = new UpdateObjectOptions { IfMetagenerationMatch = obj.Metageneration };

        var updatedObject = storage.UpdateObject(obj, options);
        Console.WriteLine($"Deleted object contexts for object {objectName} in bucket {bucketName}.");
        return updatedObject;
    }
}
// [END storage_delete_object_contexts]
