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

// [START storage_copy_file]

using Google.Cloud.Storage.V1;
using System;

public class CopyFileSample
{
    /// <summary>
    /// Creates a copy of an object.
    /// </summary>
    /// <param name="sourceBucketName">The name of the bucket containing the object to copy.</param>
    /// <param name="sourceObjectName">The name of the object to copy within the bucket.</param>
    /// <param name="destBucketName">The name of the bucket to copy the object to.</param>
    /// <param name="destObjectName">The name of the object within the destination bucket.</param>
    public void CopyFile(
        string sourceBucketName = "source-bucket-name",
        string sourceObjectName = "source-file",
        string destBucketName = "destination-bucket-name",
        string destObjectName = "destination-file-name")
    {
        var storage = StorageClient.Create();
        storage.CopyObject(sourceBucketName, sourceObjectName, destBucketName, destObjectName);

        Console.WriteLine($"Copied {sourceBucketName}/{sourceObjectName} to " + $"{destBucketName}/{destObjectName}.");
    }
}
// [END storage_copy_file]
