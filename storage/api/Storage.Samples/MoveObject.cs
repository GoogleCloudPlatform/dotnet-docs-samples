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
    /// Moves an object within a bucket.
    /// </summary>
    /// <param name="sourceBucketName">Name of the source bucket containing the object to move.</param>
    /// <param name="sourceObjectName">The name of the source object to move within the bucket.</param>
    /// <param name="destinationObjectName">The name of the new object to move to within the bucket.</param>
    public void MoveObject(
        string sourceBucketName = "source-bucket-name",
        string sourceObjectName = "source-object-name",
        string destinationObjectName = "destination-object-name")
    {
        var storage = StorageClient.Create();
        storage.MoveObject(sourceBucketName, sourceObjectName, destinationObjectName);
        Console.WriteLine($"Moved {sourceBucketName}/{sourceObjectName} to " + $"{sourceBucketName}/{destinationObjectName} within a bucket.");
    }
}
// [END storage_move_object]
