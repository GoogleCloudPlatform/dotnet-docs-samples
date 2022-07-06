﻿// Copyright 2021 Google Inc.
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

// [START storage_delete_file_archived_generation]

using Google.Cloud.Storage.V1;
using System;

public class DeleteFileArchivedGenerationSample
{
    public void DeleteFileArchivedGeneration(
        string bucketName = "your-bucket-name",
        string objectName = "your-object-name",
        long? generation = 1579287380533984)
    {
        var storage = StorageClient.Create();

        storage.DeleteObject(bucketName, objectName, new DeleteObjectOptions
        {
            Generation = generation
        });

        Console.WriteLine($"Generation ${generation} of file {objectName} was deleted from bucket {bucketName}.");
    }
}
// [END storage_delete_file_archived_generation]
