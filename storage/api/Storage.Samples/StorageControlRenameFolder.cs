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

// [START storage_control_rename_folder]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlRenameFolderSample
{
    public Folder StorageControlRenameFolder(string bucketName = "your-unique-bucket-name",
        string sourceFolderName = "your_folder_name", string targetFolderName = "target_folder_name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        string folderResourceName =
            // Set project to "_" to signify globally scoped bucket
            FolderName.FormatProjectBucketFolder("_", bucketName, sourceFolderName);

        var operation = storageControl.RenameFolder(folderResourceName, targetFolderName);
        var folder = operation.PollUntilCompleted().Result;

        Console.WriteLine($"Renamed folder {sourceFolderName} to {targetFolderName}");
        return folder;
    }
}
// [END storage_control_rename_folder]
