// Copyright 2020 Google Inc.
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

// [START storage_remove_file_owner]

using Google.Cloud.Storage.V1;
using System;
using System.Linq;

public class RemoveFileOwnerSample
{
    /// <summary>
    /// Removes a file's owner.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object within the bucket.</param>
    /// <param name="userEmail">The user email.</param>
    public void RemoveFileOwner(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        string userEmail = "dev@iam.gserviceaccount.com")
    {
        var storage = StorageClient.Create();
        var storageObject = storage.GetObject(bucketName, objectName, new GetObjectOptions { Projection = Projection.Full });
        if (null == storageObject.Acl)
            return;
        storageObject.Acl = storageObject.Acl.Where((acl) => !(acl.Entity == $"user-{userEmail}" && acl.Role == "OWNER")).ToList();
        var updatedObject = storage.UpdateObject(storageObject, new UpdateObjectOptions
        {
            // Avoid race conditions.
            IfMetagenerationMatch = storageObject.Metageneration,
        });
        Console.WriteLine($"Removed user {userEmail} from file {objectName}.");
    }
}
// [END storage_remove_file_owner]
