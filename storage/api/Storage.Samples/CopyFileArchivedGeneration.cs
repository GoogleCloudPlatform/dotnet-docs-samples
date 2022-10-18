// Copyright 2021 Google Inc.
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

// [START storage_copy_file_archived_generation]

using Google.Cloud.Storage.V1;
using System;

public class CopyFileArchivedGenerationSample
{
    public Google.Apis.Storage.v1.Data.Object CopyFileArchivedGeneration(
        string sourceBucketName = "source-bucket-name",
        string sourceObjectName = "source-file",
        string destBucketName = "destination-bucket-name",
        string destObjectName = "destination-file-name",
        long? generation = 1579287380533984)
    {
        var storage = StorageClient.Create();
        var copyOptions = new CopyObjectOptions
        {
            SourceGeneration = generation
        };

        var copiedFile = storage.CopyObject(sourceBucketName, sourceObjectName,
            destBucketName, destObjectName, copyOptions);

        Console.WriteLine($"Generation {generation} of the object {sourceBucketName}/{sourceObjectName} " +
            $"was copied to to {destBucketName}/{destObjectName}.");

        return copiedFile;
    }
}
// [END storage_copy_file_archived_generation]
