using Google.Apis.Storage.v1.Data;
using Google.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        private static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
                "Usage: \n" +
                "  QuickStart create [new-bucket-name]\n" +
                "  QuickStart list\n" +
                "  QuickStart list bucket-name [prefix] [delimiter]\n" +
                "  QuickStart upload bucket-name local-file-path [object-name]\n" +
                "  QuickStart download bucket-name object-name [local-file-path]\n" +
                "  QuickStart delete bucket-name\n" +
                "  QuickStart delete bucket-name object-name\n";

        public QuickStart(TextWriter stdout)
        {
            _out = stdout;
        }

        readonly TextWriter _out;

        // [START storage_create_bucket]
        private void CreateBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            if (bucketName == null)
                bucketName = RandomBucketName();
            storage.CreateBucket(s_projectId, bucketName);
            _out.WriteLine($"Created {bucketName}.");
        }
        // [END storage_create_bucket]

        // [START storage_list_buckets]
        private void ListBuckets()
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListBuckets(s_projectId))
            {
                _out.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_buckets]

        // [START storage_delete_bucket]
        private void DeleteBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            storage.DeleteBucket(bucketName);
            _out.WriteLine($"Deleted {bucketName}.");
        }
        // [END storage_delete_bucket]

        // [START storage_list_files]
        private void ListObjects(string bucketName)
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListObjects(bucketName, ""))
            {
                _out.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_files]

        // [START storage_list_files_with_prefix]
        private void ListObjects(string bucketName, string prefix,
            string delimiter)
        {
            var storage = StorageClient.Create();
            var options = new ListObjectsOptions() { Delimiter = delimiter };
            foreach (var storageObject in storage.ListObjects(
                bucketName, prefix, options))
            {
                _out.WriteLine(storageObject.Name);
            }
        }
        // [END storage_list_files_with_prefix]

        // [START storage_upload_file]
        private void UploadFile(string bucketName, string localPath,
            string objectName = null)
        {
            var storage = StorageClient.Create();
            using (var f = File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                storage.UploadObject(bucketName, objectName, null, f);
                _out.WriteLine($"Uploaded {objectName}.");
            }
        }
        // [END storage_upload_file]

        // [START storage_delete_file]
        private void DeleteObject(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            storage.DeleteObject(bucketName, objectName);
            _out.WriteLine($"Deleted {objectName}.");
        }
        // [END storage_delete_file]

        // [START storage_download_file]
        private void DownloadObject(string bucketName, string objectName,
            string localPath = null)
        {
            var storage = StorageClient.Create();
            localPath = localPath ?? Path.GetFileName(objectName);
            using (var outputFile = File.OpenWrite(localPath))
            {
                storage.DownloadObject(bucketName, objectName, outputFile);
            }
            _out.WriteLine($"downloaded {objectName} to {localPath}.");
        }
        // [END storage_download_file]

        // [START storage_get_metadata]
        private void GetMetadata(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);
            _out.WriteLine($"Bucket:\t{storageObject.Bucket}");
            _out.WriteLine($"CacheControl:\t{storageObject.CacheControl}");
            _out.WriteLine($"ComponentCount:\t{storageObject.ComponentCount}");
            _out.WriteLine($"ContentDisposition:\t{storageObject.ContentDisposition}");
            _out.WriteLine($"ContentEncoding:\t{storageObject.ContentEncoding}");
            _out.WriteLine($"ContentLanguage:\t{storageObject.ContentLanguage}");
            _out.WriteLine($"ContentType:\t{storageObject.ContentType}");
            _out.WriteLine($"Crc32c:\t{storageObject.Crc32c}");
            _out.WriteLine($"ETag:\t{storageObject.ETag}");
            _out.WriteLine($"Generation:\t{storageObject.Generation}");
            _out.WriteLine($"Id:\t{storageObject.Id}");
            _out.WriteLine($"Kind:\t{storageObject.Kind}");
            _out.WriteLine($"KmsKeyName:\t{storageObject.KmsKeyName}");
            _out.WriteLine($"Md5Hash:\t{storageObject.Md5Hash}");
            _out.WriteLine($"MediaLink:\t{storageObject.MediaLink}");
            _out.WriteLine($"Metageneration:\t{storageObject.Metageneration}");
            _out.WriteLine($"Name:\t{storageObject.Name}");
            _out.WriteLine($"Size:\t{storageObject.Size}");
            _out.WriteLine($"StorageClass:\t{storageObject.StorageClass}");
            _out.WriteLine($"TimeCreated:\t{storageObject.TimeCreated}");
            _out.WriteLine($"Updated:\t{storageObject.Updated}");
        }
        // [END storage_get_metadata]

        /// <summary>
        /// Delete all the files in a bucket, then delete the bucket.
        /// </summary>
        /// <param name="bucketName"></param>
        private async Task NukeBucketAsync(string bucketName)
        {
            var storage = StorageClient.Create();
            var objectList = await storage.ListObjectsAsync(bucketName, "").ToArray();
            var deleteTasks = new Task[objectList.Length];
            for (int i = 0; i < objectList.Length; ++i)
            {
                deleteTasks[i] = storage.DeleteObjectAsync(bucketName,
                    objectList[i].Name);
                _out.WriteLine($"Deleting {objectList[i].Name}.");
            }
            Task.WaitAll(deleteTasks);
            await storage.DeleteBucketAsync(bucketName);
            _out.WriteLine($"Deleted {bucketName}.");
        }

        public bool PrintUsage()
        {
            _out.WriteLine(s_usage);
            return true;
        }

        public static int Main(string[] args)
        {
            QuickStart quickStart = new QuickStart(Console.Out);
            return quickStart.Run(args);
        }

        public int Run(string[] args)
        {
            if (s_projectId == "YOUR-PROJECT" + "-ID")
            {
                _out.WriteLine("Update program.cs and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.");
                return -1;
            }
            if (args.Length < 1 && PrintUsage()) return -1;
            try
            {
                switch (args[0].ToLower())
                {
                    case "create":
                        CreateBucket(args.Length < 2 ? null : args[1]);
                        break;

                    case "list":
                        if (args.Length < 2)
                            ListBuckets();
                        else if (args.Length < 3)
                            ListObjects(args[1]);
                        else
                            ListObjects(args[1], args[2],
                                args.Length < 4 ? null : args[3]);
                        break;

                    case "delete":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        if (args.Length < 3)
                        {
                            DeleteBucket(args[1]);
                        }
                        else
                        {
                            DeleteObject(args[1], args[2]);
                        }
                        break;

                    case "upload":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        UploadFile(args[1], args[2], args.Length < 4 ? null : args[3]);
                        break;

                    case "download":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        DownloadObject(args[1], args[2], args.Length < 4 ? null : args[3]);
                        break;

                    case "get-metadata":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GetMetadata(args[1], args[2]);
                        break;

                    case "nuke":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        Task.Run(() => NukeBucketAsync(args[1]));
                        break;

                    default:
                        PrintUsage();
                        return -1;
                }
                return 0;
            }
            catch (Google.GoogleApiException e)
            {
                _out.WriteLine(e.Message);
                return e.Error.Code;
            }
        }

        private static string RandomBucketName()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                string legalChars = "abcdefhijklmnpqrstuvwxyz";
                byte[] randomByte = new byte[1];
                var randomChars = new char[20];
                int nextChar = 0;
                while (nextChar < randomChars.Length)
                {
                    rng.GetBytes(randomByte);
                    if (legalChars.Contains((char)randomByte[0]))
                        randomChars[nextChar++] = (char)randomByte[0];
                }
                return new string(randomChars);
            }
        }
    }
}
