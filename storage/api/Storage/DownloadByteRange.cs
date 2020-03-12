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

using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Net.Http;

namespace Storage
{
    public class DownloadByteRange
    {
        // [START storage_download_byte_range]
        public static void StorageDownloadByteRange(string bucketName, string objectName,
            long firstByte, long lastByte, string localPath = null)
        {
            var storageClient = StorageClient.Create();
            localPath = localPath ??
                $"{Path.GetFileName(objectName)}_{firstByte}-{lastByte}";

            // Create an HTTP request for the media, for a limited byte range.
            StorageService storage = storageClient.Service;
            var uri = new Uri(
                $"{storage.BaseUri}b/{bucketName}/o/{objectName}?alt=media");
            var request = new HttpRequestMessage() { RequestUri = uri };
            request.Headers.Range =
                new System.Net.Http.Headers.RangeHeaderValue(firstByte,
                lastByte);
            using (var outputFile = File.OpenWrite(localPath))
            {
                // Use the HttpClient in the storage object because it supplies
                // all the authentication headers we need.
                var response = storage.HttpClient.SendAsync(request).Result;
                response.Content.CopyToAsync(outputFile, null).Wait();
                Console.WriteLine($"downloaded {objectName} to {localPath}.");
            }
        }
        // [END storage_download_byte_range]
    }
}
