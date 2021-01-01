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
using System.IO;

public class ComposeObjectSample
{
    public void ComposeObject(string bucketName, string objectName, string[] files)
    {
        var storage = StorageClient.Create();

        var sourceObjects = new List<ComposeRequest.SourceObjectsData>();
        foreach (var fileName in files)
        {
            sourceObjects.Add(new ComposeRequest.SourceObjectsData
            {
                Name = Path.GetFileName(fileName)
            });
        };

        storage.Service.Objects.Compose(new ComposeRequest
        {
            SourceObjects = sourceObjects,
            Destination = new Google.Apis.Storage.v1.Data.Object
            {
                ContentType = "text/plain"
            }
        }, bucketName, objectName).Execute();

        Console.WriteLine($"New composite files {objectName} was created by combining {string.Join(",", files)}.");
    }
}
// [END storage_compose_file]
