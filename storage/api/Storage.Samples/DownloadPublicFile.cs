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

// [START storage_download_public_file]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

public class DownloadPublicFileSample
{
    public string DownloadPublicFile(
        string bucketName = "your-bucket-name",
        string objectName = "your-object-name",
        string localPath = "path/to/download/object/to")
    {
        var storage = StorageClient.CreateUnauthenticated();

        using var outputFile = File.OpenWrite(localPath);
        storage.DownloadObject(bucketName, objectName, outputFile);

        Console.WriteLine($"Downloaded public file {objectName} from bucket {bucketName} to {localPath}.");
        return localPath;
    }
}
// [END storage_download_public_file]
