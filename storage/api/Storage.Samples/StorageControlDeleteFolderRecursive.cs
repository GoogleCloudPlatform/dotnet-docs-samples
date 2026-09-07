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

// [START storage_control_delete_folder_recursive]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlDeleteFolderRecursiveSample
{
    public void StorageControlDeleteFolderRecursive(string bucketName = "your-unique-bucket-name", string folderName = "your_folder_name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        // Set project to "_" to signify globally scoped bucket
        string folderResourceName = FolderName.FormatProjectBucketFolder("_", bucketName, folderName);

        // Execute the long-running operation and wait for completion
        storageControl.DeleteFolderRecursive(folderResourceName).PollUntilCompleted();

        Console.WriteLine($"Deleted folder {folderResourceName}");
    }
}
// [END storage_control_delete_folder_recursive]
