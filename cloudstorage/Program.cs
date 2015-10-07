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
// [START all]
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GCSSearch
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + err.Message);
                }
            }
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private const int KB = 0x400;
        private const int DownloadChunkSize = 256 * KB;

        private async Task Run()
        {
            // Enter values here so you don't have to type them every time.
            string projectId = @"";
            string bucketName = @"";

            if (String.IsNullOrWhiteSpace(projectId))
            {
                Console.Write("Enter your project id: ");
                projectId = Console.ReadLine().Trim();
            }
            if (String.IsNullOrWhiteSpace(bucketName))
            {
                Console.Write("Enter your bucket name: ");
                bucketName = Console.ReadLine().Trim();
            }

            GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[] { StorageService.Scope.DevstorageReadWrite });
            }

            StorageService service = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GCS Sample",
            });

            Console.WriteLine("List of buckets in current project");
            Buckets buckets = await service.Buckets.List(projectId).ExecuteAsync();

            foreach (var bucket in buckets.Items)
            {
                Console.WriteLine(bucket.Name);
            }

            Console.WriteLine("Total number of items in bucket: " + buckets.Items.Count);
            Console.WriteLine("=============================");

            // using Google.Apis.Storage.v1.Data.Object to disambiguate from System.Object
            Google.Apis.Storage.v1.Data.Object fileobj = 
                new Google.Apis.Storage.v1.Data.Object() { Name = "somefile.txt" };

            Console.WriteLine("Creating " + fileobj.Name + " in bucket " + bucketName);
            byte[] msgtxt = Encoding.UTF8.GetBytes("Lorem Ipsum");

            await service.Objects.Insert(fileobj, bucketName, new MemoryStream(msgtxt),
                "text/plain").UploadAsync();

            Console.WriteLine("Object created: " + fileobj.Name);

            Console.WriteLine("=============================");

            Console.WriteLine("Reading object " + fileobj.Name + " in bucket: " + bucketName);
            var req = service.Objects.Get(bucketName, fileobj.Name);
            Google.Apis.Storage.v1.Data.Object readobj = await req.ExecuteAsync();

            Console.WriteLine("Object MediaLink: " + readobj.MediaLink);

            // download using Google.Apis.Download and display the progress
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var fileName = Path.Combine(pathUser, "Downloads") + "\\" + readobj.Name;
            Console.WriteLine("Starting download to " + fileName);
            var downloader = new MediaDownloader(service) { ChunkSize = DownloadChunkSize };
            // add a delegate for the progress changed event for writing to console on changes
            downloader.ProgressChanged += progress =>
                Console.WriteLine(progress.Status + " " + progress.BytesDownloaded + " bytes");

            using (var fileStream = new System.IO.FileStream(fileName,
                System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var progress = await downloader.DownloadAsync(readobj.MediaLink, fileStream);
                if (progress.Status == DownloadStatus.Completed)
                {
                    Console.WriteLine(readobj.Name + " was downloaded successfully");
                }
                else
                {
                    Console.WriteLine("Download {0} was interrupted. Only {1} were downloaded. ",
                        readobj.Name, progress.BytesDownloaded);
                }
            }
            Console.WriteLine("=============================");
        }
    }
}
// [END all]