/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * Sample code to issue several basic Google Cloud Store (GCS) operations
 * using the Google Client Libraries.
 * For more information, see documentation for Compute Storage .NET client
 * https://cloud.google.com/storage/docs/json_api/v1/json-api-dotnet-samples
 */

using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.IO;
using System.Text;

namespace GoogleCloudSamples
{
    public class StorageSample
    {
        // [START create_storage_client]
        public StorageService CreateStorageClient()
        {
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
            credentials = credentials.CreateScoped(new[] { StorageService.Scope.DevstorageFullControl });

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = "Storage Sample",
                HttpClientInitializer = credentials
            };

            return new StorageService(serviceInitializer);
        }
        // [END create_storage_client]

        // [START list_buckets]
        public void ListBuckets(string projectId)
        {
            StorageService storage = CreateStorageClient();

            var buckets = storage.Buckets.List(projectId).Execute();

            if (buckets.Items != null)
            {
                foreach (var bucket in buckets.Items)
                {
                    Console.WriteLine($"Bucket: {bucket.Name}");
                }
            }
        }
        // [END list_buckets]

        // [START list_objects]
        public void ListObjects(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            var objects = storage.Objects.List(bucketName).Execute();

            if (objects.Items != null)
            {
                foreach (var obj in objects.Items)
                {
                    Console.WriteLine($"Object: {obj.Name}");
                }
            }
        }
        // [END list_objects]

        // [START upload_stream]
        public void UploadStream(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            var content = "My text object content";
            var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            storage.Objects.Insert(
                bucket: bucketName,
                stream: uploadStream,
                contentType: "text/plain",
                body: new Google.Apis.Storage.v1.Data.Object() { Name = "text-file.txt" }
            ).Upload();

            Console.WriteLine("Uploaded text-file.txt");
        }
        // [END upload_stream]

        // [START download_stream]
        public void DownloadStream(string bucketName)
        {
            StorageService storage = CreateStorageClient();

            using (var stream = new MemoryStream())
            {
                storage.Objects.Get(bucketName, "text-file.txt").Download(stream);

                var content = Encoding.UTF8.GetString(stream.GetBuffer());

                Console.WriteLine($"Downloaded text-file.txt with content: {content}");
            }
        }
        // [END download_stream]
    }
}