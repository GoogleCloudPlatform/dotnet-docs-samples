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

// [START storage_control_delete_folder]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlDeleteFolderSample
{
    public void StorageControlDeleteFolder(string bucketName = "your-unique-bucket-name",
        string folderName = "your_folder_name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        string folderResourceName =
            // Set project to "_" to signify globally scoped bucket
            FolderName.FormatProjectBucketFolder("_", bucketName, folderName);

        storageControl.DeleteFolder(folderResourceName);

        Console.WriteLine($"Deleted folder {folderName} from bucket {bucketName}");
    }
}
// [END storage_control_delete_folder]
