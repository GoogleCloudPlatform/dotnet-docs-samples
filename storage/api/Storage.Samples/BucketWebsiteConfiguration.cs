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

// [START storage_define_bucket_website_configuration]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketWebsiteConfigurationSample
{
    public Bucket BucketWebsiteConfiguration(
        string bucketName = "your-bucket-name",
        string mainPageSuffix = "index.html",
        string notFoundPage = "404.html")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.Website == null)
        {
            bucket.Website = new Bucket.WebsiteData();
        }
        bucket.Website.MainPageSuffix = mainPageSuffix;
        bucket.Website.NotFoundPage = notFoundPage;

        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"Static website bucket {bucketName} is set up to use {mainPageSuffix} as the index page and {notFoundPage} as the 404 not found page.");
        return bucket;
    }
}
// [END storage_define_bucket_website_configuration]
