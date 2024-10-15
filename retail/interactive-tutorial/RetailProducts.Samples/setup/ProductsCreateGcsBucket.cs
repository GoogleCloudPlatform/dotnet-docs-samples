// Copyright 2022 Google Inc.

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
using System;
using System.IO;

public static class ProductsCreateGcsBucket
{
    private const string ProductFileName = "products.json";
    private const string InvalidProductFileName = "products_some_invalid.json";

    private static readonly string requestTimeStamp = DateTime.UtcNow.ToString("ddMMyyyyhhmmss");

    private static readonly string productFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{ProductFileName}");
    private static readonly string invalidProductFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{InvalidProductFileName}");
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string productsBucketName = $"{projectId}_products_{requestTimeStamp}";

    /// <summary>
    /// Create products GCS bucket with data.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformCreationOfGcsBucket()
    {
        // Create a GCS bucket.
        Bucket createdProductsBucket = CreateTestResources.CreateBucket(productsBucketName);

        // Upload products.json file to a bucket.
        CreateTestResources.UploadBlob(createdProductsBucket.Name, productFilePath, ProductFileName);

        // Upload products_some_invalid.json file to a bucket.
        CreateTestResources.UploadBlob(createdProductsBucket.Name, invalidProductFilePath, InvalidProductFileName);
    }
}
