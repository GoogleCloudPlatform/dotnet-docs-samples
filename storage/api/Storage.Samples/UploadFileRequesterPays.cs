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

// [START storage_upload_requester_pays]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

public class UploadFileRequesterPaysSample
{
    /// <summary>
    /// Uploads object with requester pays.
    /// </summary>
    /// <param name="projectId">ID of the project which will be billed for the request.</param>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="localPath">The local path.</param>
    /// <param name="objectName">The name of the object within the bucket.</param>
    public void UploadFileRequesterPays(string projectId = "your-project-id", string bucketName = "your-unique-bucket-name", string localPath = "path/to/your/file", string objectName = null)
    {
        var storage = StorageClient.Create();
        using (var f = File.OpenRead(localPath))
        {
            objectName = objectName ?? Path.GetFileName(localPath);
            storage.UploadObject(bucketName, objectName, null, f, new UploadObjectOptions()
            {
                UserProject = projectId,
            });
            Console.WriteLine($"Uploaded {objectName}.");
        }
    }
}
// [END storage_upload_requester_pays]
