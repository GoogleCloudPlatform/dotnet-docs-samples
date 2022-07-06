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

// [START storage_file_upload_from_memory]

using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Text;

public class UploadObjectFromMemorySample
{
    public void UploadObjectFromMemory(
        string bucketName = "unique-bucket-name",
        string objectName = "file-name",
        string contents = "Hello world!")
    {
        var storage = StorageClient.Create();
        byte[] byteArray = Encoding.UTF8.GetBytes(contents);
        MemoryStream stream = new MemoryStream(byteArray);
        storage.UploadObject(bucketName, objectName, "application/octet-stream" , stream);

        Console.WriteLine($" {objectName} uploaded to bucket {bucketName} with contents: {contents}");
    }
}

// [END storage_file_upload_from_memory]
