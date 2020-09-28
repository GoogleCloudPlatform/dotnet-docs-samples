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

// [START storage_download_byte_range]

using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class DownloadByteRangeAsyncSample
{
    public async Task DownloadByteRangeAsync(
        string bucketName = "your-unique-bucket-name",
        string objectName = "my-file-name",
        long firstByte = 0,
        long lastByte = 20,
        string localPath = "my-local-path/my-file-name")
    {
        var storageClient = StorageClient.Create();

        // Create an HTTP request for the media, for a limited byte range.
        StorageService storage = storageClient.Service;
        var uri = new Uri($"{storage.BaseUri}b/{bucketName}/o/{objectName}?alt=media");

        var request = new HttpRequestMessage { RequestUri = uri };
        request.Headers.Range = new RangeHeaderValue(firstByte, lastByte);

        using var outputFile = File.OpenWrite(localPath);
        // Use the HttpClient in the storage object because it supplies
        // all the authentication headers we need.
        var response = await storage.HttpClient.SendAsync(request);
        await response.Content.CopyToAsync(outputFile, null);
        Console.WriteLine($"Downloaded {objectName} to {localPath}.");
    }
}
// [END storage_download_byte_range]
