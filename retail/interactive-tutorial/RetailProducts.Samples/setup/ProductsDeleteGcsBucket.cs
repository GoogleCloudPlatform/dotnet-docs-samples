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

public static class ProductsDeleteGcsBucket
{
    private static readonly StorageClient storageClient = StorageClient.Create();

    /// <summary>Delete bucket.</summary>
    private static void DeleteBucket(string bucketName)
    {
        Console.WriteLine($"Deleting Bucket {bucketName}.");

        try
        {
            var bucketToDelete = storageClient.GetBucket(bucketName);
            DeleteObjectsFromBucket(bucketToDelete);
            storageClient.DeleteBucket(bucketToDelete.Name);
            Console.WriteLine($"Bucket {bucketToDelete.Name} is deleted.");
        }
        catch (Exception)
        {
            Console.WriteLine($"Bucket {bucketName} does not exist.");
        }
    }

    /// <summary>Delete all objects from bucket.</summary>
    private static void DeleteObjectsFromBucket(Bucket bucket)
    {
        Console.WriteLine($"Deleting object from bucket {bucket.Name}.");

        var blobs = storageClient.ListObjects(bucket.Name);
        foreach (var blob in blobs)
        {
            storageClient.DeleteObject(bucket.Name, blob.Name);
        }

        Console.WriteLine($"All objects are deleted from a GCS bucket {bucket.Name}.");
    }

    /// <summary>
    /// Delete test resources.
    /// </summary>
    public static void PerformDeletionOfProductsGcsBucket(string bucketName)
    {
        string productsBucketName = bucketName ?? Environment.GetEnvironmentVariable("BUCKET_NAME");

        DeleteBucket(productsBucketName);
    }
}