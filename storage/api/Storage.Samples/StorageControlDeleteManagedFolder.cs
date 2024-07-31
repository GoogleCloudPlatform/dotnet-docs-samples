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

// [START storage_control_managed_folder_delete]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlDeleteManagedFolderSample
{
    public void StorageControlDeleteManagedFolder(string bucketId = "your-unique-bucket-name",
        string managedFolderId = "your_managed_folder_id")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        ManagedFolderName managedFolderResourceName =
            // Set project to "_" to signify globally scoped bucket
            new ManagedFolderName("_", bucketId, managedFolderId);

        storageControl.DeleteManagedFolder(managedFolderResourceName);

        Console.WriteLine($"Deleted managed folder {managedFolderId} from bucket {bucketId}");
    }
}
// [END storage_control_managed_folder_delete]
