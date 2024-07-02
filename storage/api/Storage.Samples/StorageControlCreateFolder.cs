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

// [START storage_control_create_folder]
using Google.Cloud.Storage.Control.V2;
using System;

public class StorageControlCreateFolderSample
{
    public Folder StorageControlCreateFolder(string bucketName = "your-unique-bucket-name",
        string folderName = "your_folder_name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        var request = new CreateFolderRequest
        {
            // Set project to "_" to signify globally scoped bucket
            Parent = BucketName.FormatProjectBucket("_", bucketName),
            FolderId = folderName
        };

        Folder folder = storageControl.CreateFolder(request);

        Console.WriteLine($"Folder {folderName} created in bucket {bucketName}");
        return folder;
    }
}
// [END storage_control_create_folder]

