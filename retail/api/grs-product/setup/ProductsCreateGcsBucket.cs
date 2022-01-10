// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace grs_product.setup
{
    public static class ProductsCreateGcsBucket
    {
        private const string FileName = "products.json";
        private const string InvalidFileName = "products_some_invalid.json";

        private static readonly string ProjectId = Environment.GetEnvironmentVariable("PROJECT_ID");

        // The request timestamp
        private static readonly string RequestTimeStamp = DateTime.Now.ToUniversalTime().ToString("ddMMyyyyhhmmss");

        // The outdated request timestamp
        // request_time = datetime.datetime.now() - datetime.timedelta(days=1)

        private static readonly string BucketName = $"{ProjectId}_{RequestTimeStamp}";
        private static readonly string FilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"grs-product/resources/{FileName}");
        private static readonly string InvalidFilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"grs-product/resources/{InvalidFileName}");

        private static Bucket CreateBucket(string bucketName)
        {
            var newBucket = new Bucket();
            Console.WriteLine($"\nBucket name: {bucketName}\n");
            var bucketsInYourProject = ListBuckets();
            var bucketNamesInYourProject = bucketsInYourProject.Select(x => x.Name).ToArray();
            if (bucketNamesInYourProject.Contains(bucketName))
            {
                Console.WriteLine($"\nBucket {bucketName} already exists.\n");
            }
            else
            {
                var storageClient = StorageClient.Create();
                var bucket = new Bucket
                {
                    Name = bucketName,
                    StorageClass = "STANDARD",
                    Location = "us"
                };

                newBucket = storageClient.CreateBucket(ProjectId, bucket);

                Console.WriteLine($"\nCreated bucket {newBucket.Name} in {newBucket.Location} with storage class {newBucket.StorageClass}\n");
            }
            return newBucket;
        }

        private static List<Bucket> ListBuckets()
        {
            var bucketsList = new List<Bucket>();
            var storageClient = StorageClient.Create();
            var buckets = storageClient.ListBuckets(ProjectId);

            foreach (var bucket in buckets)
            {
                bucketsList.Add(bucket);
                Console.WriteLine(bucket.Name);
            }

            return bucketsList;
        }

        private static void UploadBlob(string bucketName, string localPath, string objectName)
        {
           
            var storageClient = StorageClient.Create();
            var bucket = storageClient.GetBucket(bucketName);

            using var fileStream = File.OpenRead(localPath);
            storageClient.UploadObject(bucketName, objectName, null, fileStream);
            Console.WriteLine($"Uploaded {objectName}.");
        }

        [Attributes.Example]
        public static void PerformCreationOfGcsBucket()
        {
            CreateBucket(BucketName);
            UploadBlob(BucketName, FilePath, FileName);
            UploadBlob(BucketName, InvalidFilePath, InvalidFileName);

            Console.WriteLine($"\nThe gcs bucket {BucketName} was created");
        }
    }
}
