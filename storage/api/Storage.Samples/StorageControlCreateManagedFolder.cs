// Copyright 2024 Google LLC
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

// [START storage_control_managed_folder_create]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlCreateManagedFolderSample
{
    public ManagedFolder StorageControlCreateManagedFolder(string bucketId = "your-unique-bucket-name",
        string managedFolderId = "your_managed_folder_id")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        ManagedFolder managedFolder = storageControl.CreateManagedFolder(
            // Set project to "_" to signify globally scoped bucket
            new BucketName("_", bucketId),
            new ManagedFolder(),
            managedFolderId
        );

        Console.WriteLine($"Managed Folder {managedFolderId} created in bucket {bucketId}");
        return managedFolder;
    }
}
// [END storage_control_managed_folder_create]
