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
    public void ComposeObject(
        string bucketName = "your-bucket-name",
        string firstObjectName = "your-first-object-name",
        string secondObjectName = "your-second-object-name",
        string targetObjectName = "new-composite-object-name")
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
            SourceObjects = sourceObjects,
            Destination = new Google.Apis.Storage.v1.Data.Object { ContentType = "text/plain" }
        }, bucketName, targetObjectName).Execute();

        Console.WriteLine($"New composite file {targetObjectName} was created in bucket {bucketName}" +
            $" by combining {firstObjectName} and {secondObjectName}.");
    }
}
// [END storage_compose_file]
