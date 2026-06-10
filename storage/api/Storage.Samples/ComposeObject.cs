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

// [START storage_compose_file]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;

public class ComposeObjectSample
{
    /// <summary>
    /// Combines multiple source objects into a single target object within a specified bucket,
    /// with the option to delete the original source objects upon successful composition.
    /// </summary>
    /// <param name="bucketName">The name of the bucket containing the source objects.</param>
    /// <param name="firstObjectName">The name of the first source object to be composed.</param>
    /// <param name="secondObjectName">The name of the second source object to be composed.</param>
    /// <param name="targetObjectName">The name for the newly created composite object.</param>
    /// <param name="deleteSourceObjects">If set to <c>true</c>, the method will automatically delete <paramref name="firstObjectName"/> and <paramref name="secondObjectName"/> upon successful composition.
    /// Defaults to <c>false</c>.</param>
    public void ComposeObject(
        string bucketName = "your-bucket-name",
        string firstObjectName = "your-first-object-name",
        string secondObjectName = "your-second-object-name",
        string targetObjectName = "new-composite-object-name",
        bool deleteSourceObjects = false)
    {
        var storage = StorageClient.Create();

        var sourceObjects = new List<ComposeRequest.SourceObjectsData>
        {
            new ComposeRequest.SourceObjectsData { Name = firstObjectName },
            new ComposeRequest.SourceObjectsData { Name = secondObjectName }
        };
        //You could add as many sourceObjects as you want here, up to the max of 32.

        storage.Service.Objects.Compose(new ComposeRequest
        {
            DeleteSourceObjects = deleteSourceObjects,
            SourceObjects = sourceObjects,
            Destination = new Google.Apis.Storage.v1.Data.Object { ContentType = "text/plain" }
        }, bucketName, targetObjectName).Execute();

        string deletionMessage = deleteSourceObjects ? " and the source objects were deleted." : ".";
        Console.WriteLine($"New composite file {targetObjectName} was created in bucket {bucketName}" +
            $" by combining {firstObjectName} and {secondObjectName}{deletionMessage}");
    }
}
// [END storage_compose_file]
