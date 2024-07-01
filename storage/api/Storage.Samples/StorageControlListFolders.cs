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

// [START storage_control_list_folders]
using Google.Cloud.Storage.Control.V2;
using System;
using System.Collections.Generic;

public class StorageControlListFoldersSample
{
    public IEnumerable<Folder> StorageControlListFolders(string bucketName = "your-unique-bucket-name")
    {
        StorageControlClient storageControl = StorageControlClient.Create();

        // Use "_" for project ID to signify globally scoped bucket
        string bucketResourceName = BucketName.FormatProjectBucket("_", bucketName);
        var folders = storageControl.ListFolders(bucketResourceName);

        foreach (var folder in folders)
        {
            Console.Write(folder.Name);
        }
        return folders;
    }
}
// [END storage_control_list_folders]
