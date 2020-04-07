﻿// Copyright 2020 Google Inc.
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

// [START storage_download_encrypted_file]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

namespace Storage
{
    public class DownloadEncryptedFile
    {
        public static void StorageDownloadEncryptedFile(string key, string bucketName,
            string objectName, string localPath = null)
        {
            var storage = StorageClient.Create();
            localPath = localPath ?? Path.GetFileName(objectName);
            using (var outputFile = File.OpenWrite(localPath))
            {
                storage.DownloadObject(bucketName, objectName, outputFile,
                    new DownloadObjectOptions()
                    {
                        EncryptionKey = EncryptionKey.Create(
                            Convert.FromBase64String(key))
                    });
            }
            Console.WriteLine($"downloaded {objectName} to {localPath}.");
        }
    }
}
// [END storage_download_encrypted_file]
