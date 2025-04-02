// Copyright 2025 Google LLC
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

// [START storage_move_object]

using Google.Cloud.Storage.V1;
using System;

public class MoveObjectSample
{
    /// <summary>
    /// Sample that moves the source object to the destination object within a hierarchical namespace enabled bucket.
    /// </summary>
    /// <param name="sourceBucketName">The name of the source bucket.</param>
    /// <param name="sourceObjectName">The name of source object to move.</param>
    /// <param name="destinationObjectName">The name of destination object.</param>
    public void MoveObject(
        string sourceBucketName = "source-bucket-name",
        string sourceObjectName = "source-file",
        string destinationObjectName = "destination-file-name")
    {
        var storage = StorageClient.Create();
        storage.MoveObject(sourceBucketName, sourceObjectName, destinationObjectName);

        Console.WriteLine($"Moved {sourceBucketName}/{sourceObjectName} to " + $"{sourceBucketName}/{destinationObjectName} within a hierarchical namespace enabled bucket.");
    }

    /// <summary>
    /// Sample that moves the source object to the destination object within a hierarchical namespace enabled bucket with preconditions set.
    /// </summary>
    /// <param name="sourceBucketName">The name of the source bucket.</param>
    /// <param name="sourceObjectName">The name of source object to move.</param>
    /// <param name="destinationObjectName">The name of destination object.</param>
    /// <param name="generation">The generation of the object.</param>
    public void MoveObjectWithOptions(
       string sourceBucketName = "source-bucket-name",
       string sourceObjectName = "source-file",
       string destinationObjectName = "destination-file-name",
       long? generation = 1579287380533984)
    {
        var storage = StorageClient.Create();
        var moveOptions = new MoveObjectOptions
        {
            IfGenerationMatch = generation
        };
        storage.MoveObject(sourceBucketName, sourceObjectName, destinationObjectName, moveOptions);

        Console.WriteLine($"Moved {sourceBucketName}/{sourceObjectName} to " + $"{sourceBucketName}/{destinationObjectName} within a hierarchical namespace enabled bucket with preconditions set.");
    }
}
// [END storage_move_object]
