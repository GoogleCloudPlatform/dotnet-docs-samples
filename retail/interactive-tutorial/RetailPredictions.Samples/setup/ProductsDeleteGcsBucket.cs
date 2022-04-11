using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

namespace RetailPredictions.Samples.setup
{
    public class ProductsDeleteGcsBucket
    {
        private static readonly StorageClient storageClient = StorageClient.Create();

        /// <summary>Delete bucket.</summary>
        private static void DeleteBucket(string bucketName)
        {
            Console.WriteLine($"Deleting Bucket {bucketName}");

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
            Console.WriteLine($"Deleting object from bucket {bucket.Name}");

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
        [Runner.Attributes.Example]
        public static void PerformDeletionOfProductsGcsBucket(string bucketName)
        {
            string productsBucketName = bucketName ?? Environment.GetEnvironmentVariable("BUCKET_NAME");

            DeleteBucket(productsBucketName);
        }
    }
}
