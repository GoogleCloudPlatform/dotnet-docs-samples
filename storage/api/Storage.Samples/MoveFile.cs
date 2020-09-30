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

// [START storage_move_file]

using Google.Cloud.Storage.V1;
using System;

public class MoveFileSample
{
    public void MoveFile(
        string sourceBucketName = "your-unique-bucket-name",
        string sourceObjectName = "your-object-name",
        string targetBucketName = "target-object-bucket",
        string targetObjectName = "target-object-name")
    {
        var storage = StorageClient.Create();
        storage.CopyObject(sourceBucketName, sourceObjectName, targetBucketName, targetObjectName);
        storage.DeleteObject(sourceBucketName, sourceObjectName);
        Console.WriteLine($"Moved {sourceObjectName} to {targetObjectName}.");
    }
}
// [END storage_move_file]
