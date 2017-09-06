// Copyright 2016 Google Inc.
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
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

namespace GoogleCloudSamples
{
    public class Storage
    {
        private static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
            "Usage: \n" +
            "  Storage create [new-bucket-name]\n" +
            "  Storage list\n" +
            "  Storage list bucket-name [prefix] [delimiter]\n" +
            "  Storage get-metadata bucket-name object-name\n" +
            "  Storage make-public bucket-name object-name\n" +
            "  Storage upload [-key encryption-key] bucket-name local-file-path [object-name]\n" +
            "  Storage copy source-bucket-name source-object-name dest-bucket-name dest-object-name\n" +
            "  Storage move bucket-name source-object-name dest-object-name\n" +
            "  Storage download [-key encryption-key] bucket-name object-name [local-file-path]\n" +
            "  Storage download-byte-range bucket-name object-name range-begin range-end [local-file-path]\n" +
            "  Storage generate-signed-url bucket-name object-name\n" +
            "  Storage view-bucket-iam-members bucket-name\n" +
            "  Storage add-bucket-iam-member bucket-name member\n" +
            "  Storage remove-bucket-iam-member bucket-name role member\n" +
            "  Storage print-acl bucket-name\n" +
            "  Storage print-acl bucket-name object-name\n" +
            "  Storage add-owner bucket-name user-email\n" +
            "  Storage add-owner bucket-name object-name user-email\n" +
            "  Storage add-default-owner bucket-name user-email\n" +
            "  Storage remove-owner bucket-name user-email\n" +
            "  Storage remove-owner bucket-name object-name user-email\n" +
            "  Storage remove-default-owner bucket-name user-email\n" +
            "  Storage delete bucket-name\n" +
            "  Storage delete bucket-name object-name [object-name]\n" +
            "  Storage enable-requester-pays bucket-name\n" +
            "  Storage disable-requester-pays bucket-name\n" +
            "  Storage get-requester-pays bucket-name\n" +
            "  Storage generate-encryption-key";

        // [START storage_create_bucket]
        private void CreateBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            storage.CreateBucket(s_projectId, bucketName);
            Console.WriteLine($"Created {bucketName}.");
        }
        // [END storage_create_bucket]

        // [START storage_list_buckets]
        private void ListBuckets()
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListBuckets(s_projectId))
            {
                Console.WriteLine(bucket.Name);
            }
        }
        // [END storage_list_buckets]

        // [START storage_delete_bucket]
        private void DeleteBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            storage.DeleteBucket(bucketName);
            Console.WriteLine($"Deleted {bucketName}.");
        }
        // [END storage_delete_bucket]

        // [START storage_list_files]
        private void ListObjects(string bucketName)
        {
            var storage = StorageClient.Create();
            foreach (var bucket in storage.ListObjects(bucketName, ""))
            {
                Console.WriteLine(bucket.Name);
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
                Console.WriteLine(storageObject.Name);
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
                Console.WriteLine($"Uploaded {objectName}.");
            }
        }
        // [END storage_upload_file]

        // [START storage_upload_encrypted_file]
        private void UploadEncryptedFile(string key, string bucketName,
            string localPath, string objectName = null)
        {
            var storage = StorageClient.Create();
            using (var f = File.OpenRead(localPath))
            {
                objectName = objectName ?? Path.GetFileName(localPath);
                storage.UploadObject(bucketName, objectName, null, f,
                    new UploadObjectOptions()
                    {
                        EncryptionKey = EncryptionKey.Create(
                        Convert.FromBase64String(key))
                    });
                Console.WriteLine($"Uploaded {objectName}.");
            }
        }
        // [END storage_upload_encrypted_file]

        // [START storage_delete_file]
        private void DeleteObject(string bucketName, IEnumerable<string> objectNames)
        {
            var storage = StorageClient.Create();
            foreach (string objectName in objectNames)
            {
                storage.DeleteObject(bucketName, objectName);
                Console.WriteLine($"Deleted {objectName}.");
            }
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
            Console.WriteLine($"downloaded {objectName} to {localPath}.");
        }
        // [END storage_download_file]

        // [START storage_download_encrypted_file]
        private void DownloadEncryptedObject(string key, string bucketName,
            string objectName, string localPath = null)
        {
            var storage = StorageClient.Create();
            localPath = localPath ?? Path.GetFileName(objectName);
            using (var outputFile = File.OpenWrite(localPath))
            {
                storage.DownloadObject(bucketName, objectName, outputFile,
                    new DownloadObjectOptions()
                    {
                        EncryptionKey = EncryptionKey.Create(
                            Convert.FromBase64String(key))
                    });
            }
            Console.WriteLine($"downloaded {objectName} to {localPath}.");
        }
        // [END storage_download_encrypted_file]

        // [START storage_download_byte_range]
        private void DownloadByteRange(string bucketName, string objectName,
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

        // [START storage_get_metadata]
        private void GetMetadata(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);
            Console.WriteLine($"Bucket:\t{storageObject.Bucket}");
            Console.WriteLine($"CacheControl:\t{storageObject.CacheControl}");
            Console.WriteLine($"ComponentCount:\t{storageObject.ComponentCount}");
            Console.WriteLine($"ContentDisposition:\t{storageObject.ContentDisposition}");
            Console.WriteLine($"ContentEncoding:\t{storageObject.ContentEncoding}");
            Console.WriteLine($"ContentLanguage:\t{storageObject.ContentLanguage}");
            Console.WriteLine($"ContentType:\t{storageObject.ContentType}");
            Console.WriteLine($"Crc32c:\t{storageObject.Crc32c}");
            Console.WriteLine($"ETag:\t{storageObject.ETag}");
            Console.WriteLine($"Generation:\t{storageObject.Generation}");
            Console.WriteLine($"Id:\t{storageObject.Id}");
            Console.WriteLine($"Kind:\t{storageObject.Kind}");
            Console.WriteLine($"Md5Hash:\t{storageObject.Md5Hash}");
            Console.WriteLine($"MediaLink:\t{storageObject.MediaLink}");
            Console.WriteLine($"Metageneration:\t{storageObject.Metageneration}");
            Console.WriteLine($"Name:\t{storageObject.Name}");
            Console.WriteLine($"Size:\t{storageObject.Size}");
            Console.WriteLine($"StorageClass:\t{storageObject.StorageClass}");
            Console.WriteLine($"TimeCreated:\t{storageObject.TimeCreated}");
            Console.WriteLine($"Updated:\t{storageObject.Updated}");
        }
        // [END storage_get_metadata]

        // [START storage_make_public]
        private void MakePublic(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName);
            storageObject.Acl = storageObject.Acl ?? new List<ObjectAccessControl>();
            storage.UpdateObject(storageObject, new UpdateObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            });
            Console.WriteLine(objectName + " is now public and can be fetched from " +
                storageObject.MediaLink);
        }
        // [END storage_make_public]

        // [START storage_move_file]
        private void MoveObject(string bucketName, string sourceObjectName,
            string destObjectName)
        {
            var storage = StorageClient.Create();
            storage.CopyObject(bucketName, sourceObjectName, bucketName,
                destObjectName);
            storage.DeleteObject(bucketName, sourceObjectName);
            Console.WriteLine($"Moved {sourceObjectName} to {destObjectName}.");
        }
        // [END storage_move_file]

        // [START storage_copy_file]
        private void CopyObject(string sourceBucketName, string sourceObjectName,
            string destBucketName, string destObjectName)
        {
            var storage = StorageClient.Create();
            storage.CopyObject(sourceBucketName, sourceObjectName,
                destBucketName, destObjectName);
            Console.WriteLine($"Copied {sourceBucketName}/{sourceObjectName} to "
                + $"{destBucketName}/{destObjectName}.");
        }
        // [END storage_copy_file]

        // [START storage_print_bucket_acl]
        private void PrintBucketAcl(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (bucket.Acl != null)
                foreach (var acl in bucket.Acl)
                {
                    Console.WriteLine($"{acl.Role}:{acl.Entity}");
                }
        }
        // [END storage_print_bucket_acl]

        // [START storage_print_bucket_default_acl]
        private void PrintBucketDefaultAcl(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (bucket.Acl != null)
                foreach (var acl in bucket.DefaultObjectAcl)
                {
                    Console.WriteLine($"{acl.Role}:{acl.Entity}");
                }
        }
        // [END storage_print_bucket_default_acl]

        // [START storage_print_bucket_acl_for_user]
        private void PrintBucketAclForUser(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });

            if (bucket.Acl != null)
                foreach (var acl in bucket.Acl.Where(
(acl) => acl.Entity == $"user-{userEmail}"))
                {
                    Console.WriteLine($"{acl.Role}:{acl.Entity}");
                }
        }
        // [END storage_print_bucket_acl_for_user]

        // [START storage_add_bucket_owner]
        private void AddBucketOwner(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (null == bucket.Acl)
            {
                bucket.Acl = new List<BucketAccessControl>();
            }
            bucket.Acl.Add(new BucketAccessControl()
            {
                Bucket = bucketName,
                Entity = $"user-{userEmail}",
                Role = "OWNER",
            });
            var updatedBucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });
        }
        // [END storage_add_bucket_owner]

        // [START storage_remove_bucket_owner]
        private void RemoveBucketOwner(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (null == bucket.Acl)
                return;
            bucket.Acl = bucket.Acl.Where((acl) =>
                !(acl.Entity == $"user-{userEmail}" && acl.Role == "OWNER")
                ).ToList();
            var updatedBucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });
        }
        // [END storage_remove_bucket_owner]

        // [START storage_add_bucket_default_owner]
        private void AddBucketDefaultOwner(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (null == bucket.Acl)
            {
                bucket.Acl = new List<BucketAccessControl>();
            }
            if (null == bucket.DefaultObjectAcl)
            {
                bucket.DefaultObjectAcl = new List<ObjectAccessControl>();
            }
            bucket.DefaultObjectAcl.Add(new ObjectAccessControl()
            {
                Bucket = bucketName,
                Entity = $"user-{userEmail}",
                Role = "OWNER",
            });
            var updatedBucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });
        }
        // [END storage_add_bucket_default_owner]

        // [START storage_remove_bucket_default_owner]
        private void RemoveBucketDefaultOwner(string bucketName, string userEmail)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName, new GetBucketOptions()
            {
                Projection = Projection.Full
            });
            if (null == bucket.DefaultObjectAcl)
                return;
            if (null == bucket.Acl)
            {
                bucket.Acl = new List<BucketAccessControl>();
            }
            bucket.DefaultObjectAcl = bucket.DefaultObjectAcl.Where((acl) =>
                 !(acl.Entity == $"user-{userEmail}" && acl.Role == "OWNER")
                ).ToList();
            var updatedBucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration,
            });
        }
        // [END storage_remove_bucket_default_owner]

        // [START view_bucket_iam_members]
        private void ViewBucketIamMembers(string bucketName)
        {
            var storage = StorageClient.Create();
            var policy = storage.GetBucketIamPolicy(bucketName);
            foreach (var binding in policy.Bindings)
            {
                Console.WriteLine($"  Role: {binding.Role}");
                Console.WriteLine("  Members:");
                foreach (var member in binding.Members)
                {
                    Console.WriteLine($"    {member}");
                }
            }
        }
        // [END view_bucket_iam_members]

        // [START add_bucket_iam_member]
        private void AddBucketIamMember(string bucketName,
            string role, string member)
        {
            var storage = StorageClient.Create();
            var policy = storage.GetBucketIamPolicy(bucketName);
            Policy.BindingsData bindingToAdd = new Policy.BindingsData();
            bindingToAdd.Role = role;
            string[] members = { member };
            bindingToAdd.Members = members;
            policy.Bindings.Add(bindingToAdd);
            storage.SetBucketIamPolicy(bucketName, policy);
            Console.WriteLine($"Added {member} with role {role} "
                + $"to {bucketName}");
        }
        // [END add_bucket_iam_member]

        // [START remove_bucket_iam_member]
        private void RemoveBucketIamMember(string bucketName,
            string role, string member)
        {
            var storage = StorageClient.Create();
            var policy = storage.GetBucketIamPolicy(bucketName);
            policy.Bindings.ToList().ForEach(response =>
            {
                if (response.Role == role)
                {
                    // Remove the role/member combo from the IAM policy.
                    response.Members = response.Members
                        .Where(m => m != member).ToList();
                    // Remove role if it contains no members.
                    if (response.Members.Count == 0)
                    {
                        policy.Bindings.Remove(response);
                    }
                }
            });
            // Set the modified IAM policy to be the current IAM policy.
            storage.SetBucketIamPolicy(bucketName, policy);
            Console.WriteLine($"Removed {member} with role {role} "
                + $"to {bucketName}");
        }
        // [END remove_bucket_iam_member]

        // [START storage_print_file_acl]
        private void PrintObjectAcl(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName,
                new GetObjectOptions() { Projection = Projection.Full });
            if (storageObject.Acl != null)
            {
                foreach (var acl in storageObject.Acl)
                {
                    Console.WriteLine($"{acl.Role}:{acl.Entity}");
                }
            }
        }
        // [END storage_print_file_acl]

        // [START storage_print_file_acl_for_user]
        private void PrintObjectAclForUser(string bucketName, string objectName,
            string userEmail)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName,
                new GetObjectOptions() { Projection = Projection.Full });
            if (storageObject.Acl != null)
            {
                foreach (var acl in storageObject.Acl
                    .Where((acl) => acl.Entity == $"user-{userEmail}"))
                {
                    Console.WriteLine($"{acl.Role}:{acl.Entity}");
                }
            }
        }
        // [END storage_print_file_acl_for_user]

        // [START storage_add_file_owner]
        private void AddObjectOwner(string bucketName, string objectName,
            string userEmail)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName,
                new GetObjectOptions() { Projection = Projection.Full });
            if (null == storageObject.Acl)
            {
                storageObject.Acl = new List<ObjectAccessControl>();
            }
            storageObject.Acl.Add(new ObjectAccessControl()
            {
                Bucket = bucketName,
                Entity = $"user-{userEmail}",
                Role = "OWNER",
            });
            var updatedObject = storage.UpdateObject(storageObject, new UpdateObjectOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = storageObject.Metageneration,
            });
        }
        // [END storage_add_file_owner]

        // [START storage_remove_file_owner]
        private void RemoveObjectOwner(string bucketName, string objectName,
            string userEmail)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(bucketName, objectName,
                new GetObjectOptions() { Projection = Projection.Full });
            if (null == storageObject.Acl)
                return;
            storageObject.Acl = storageObject.Acl.Where((acl) =>
                !(acl.Entity == $"user-{userEmail}" && acl.Role == "OWNER")
                ).ToList();
            var updatedObject = storage.UpdateObject(storageObject, new UpdateObjectOptions()
            {
                // Avoid race conditions.
                IfMetagenerationMatch = storageObject.Metageneration,
            });
        }
        // [END storage_remove_file_owner]

        // [START storage_generate_signed_url]
        private void GenerateSignedUrl(string bucketName, string objectName)
        {
            UrlSigner urlSigner = UrlSigner.FromServiceAccountPath(Environment
                .GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
            string url =
                urlSigner.Sign(bucketName, objectName, TimeSpan.FromHours(1), null);
            Console.WriteLine(url);
        }
        // [END storage_generate_signed_url]

        // [START storage_generate_encryption_key]                
        void GenerateEncryptionKey()
        {
            Console.Write(EncryptionKey.Generate().Base64Key);
        }
        // [END storage_generate_encryption_key]

        // [START storage_enable_requester_pays]
        void EnableRequesterPays(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            bucket.Billing = bucket.Billing ?? new Bucket.BillingData();
            bucket.Billing.RequesterPays = true;
            bucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Use IfMetagenerationMatch to avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration
            });
        }
        // [END storage_enable_requester_pays]

        // [START storage_disable_requester_pays]
        void DisableRequesterPays(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            bucket.Billing = bucket.Billing ?? new Bucket.BillingData();
            bucket.Billing.RequesterPays = false;
            bucket = storage.UpdateBucket(bucket, new UpdateBucketOptions()
            {
                // Use IfMetagenerationMatch to avoid race conditions.
                IfMetagenerationMatch = bucket.Metageneration
            });
        }
        // [END storage_disable_requester_pays]

        // [START storage_get_requester_pays_status]
        bool GetRequesterPays(string bucketName)
        {
            var storage = StorageClient.Create();
            var bucket = storage.GetBucket(bucketName);
            bool? requesterPaysOrNull = bucket.Billing?.RequesterPays;
            bool requesterPays = 
                requesterPaysOrNull.HasValue ? requesterPaysOrNull.Value : false;
            Console.WriteLine("RequesterPays: {0}", requesterPays);
            return requesterPays;
        }
        // [END storage_get_requester_pays_status]

        // [START storage_download_file_requester_pays]
        private void DownloadObjectRequesterPays(string bucketName, string objectName,
            string localPath = null)
        {
            var storage = StorageClient.Create();
            localPath = localPath ?? Path.GetFileName(objectName);
            using (var outputFile = File.OpenWrite(localPath))
            {
                storage.DownloadObject(bucketName, objectName, outputFile, new DownloadObjectOptions()
                {
                    UserProject = s_projectId
                });
            }
            Console.WriteLine(
                $"downloaded {objectName} to {localPath} paid by {s_projectId}.");
        }
        // [END storage_download_file_requester_pays]

        public bool PrintUsage()
        {
            Console.WriteLine(s_usage);
            return true;
        }

        public static int Main(string[] args)
        {
            Storage Storage = new Storage();
            return Storage.Run(args);
        }

        string PullFlag(string flag, ref string[] args, bool requiresValue)
        {
            string value = null;
            var newArgs = new List<string>();
            for (int i = 0; i < args.Count(); ++i)
            {
                if (flag == args[i].ToLower())
                {
                    if (requiresValue)
                    {
                        if (++i == args.Count())
                        {
                            throw new ArgumentException(
                                $"Flag {flag} requires a value.");
                        }
                    }
                    value = args[i];
                    continue;
                }
                newArgs.Add(args[i]);
            }
            args = newArgs.ToArray();
            return value;
        }

        public int Run(string[] args)
        {
            string encryptionKey;
            string requesterPays;
            if (s_projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Update program.cs and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.");
                return -1;
            }
            if (args.Length < 1 && PrintUsage()) return -1;
            try
            {
                switch (args[0].ToLower())
                {
                    case "create":
                        CreateBucket(args.Length < 2 ? RandomBucketName() : args[1]);
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
                            DeleteObject(args[1], args.Skip(2));
                        }
                        break;

                    case "upload":
                        encryptionKey = PullFlag("-key", ref args, requiresValue:true);
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (encryptionKey == null)
                        {
                            UploadFile(args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else
                        {
                            UploadEncryptedFile(encryptionKey, args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        break;

                    case "download":
                        encryptionKey = PullFlag("-key", ref args, requiresValue: true);
                        requesterPays = PullFlag("-pay", ref args, requiresValue: false);
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (encryptionKey != null)
                        {
                            DownloadEncryptedObject(encryptionKey, args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else if (requesterPays != null)
                        {
                            DownloadObjectRequesterPays(args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else
                        {
                            DownloadObject(args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        break;

                    case "download-byte-range":
                        if (args.Length < 5 && PrintUsage()) return -1;
                        DownloadByteRange(args[1], args[2],
                            long.Parse(args[3]), long.Parse(args[4]),
                            args.Length < 6 ? null : args[4]);
                        break;

                    case "get-metadata":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GetMetadata(args[1], args[2]);
                        break;

                    case "make-public":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        MakePublic(args[1], args[2]);
                        break;

                    case "move":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        MoveObject(args[1], args[2], args[3]);
                        break;

                    case "copy":
                        if (args.Length < 5 && PrintUsage()) return -1;
                        CopyObject(args[1], args[2], args[3], args[4]);
                        break;

                    case "print-acl":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        if (args.Length < 3)
                            PrintBucketAcl(args[1]);
                        else
                            PrintObjectAcl(args[1], args[2]);
                        break;

                    case "print-acl-for-user":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            PrintBucketAclForUser(args[1], args[2]);
                        else
                            PrintObjectAclForUser(args[1], args[2], args[3]);
                        break;

                    case "print-default-acl":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        PrintBucketDefaultAcl(args[1]);
                        break;

                    case "add-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            AddBucketOwner(args[1], args[2]);
                        else
                            AddObjectOwner(args[1], args[2], args[3]);
                        break;

                    case "remove-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            RemoveBucketOwner(args[1], args[2]);
                        else
                            RemoveObjectOwner(args[1], args[2], args[3]);
                        break;

                    case "add-default-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        AddBucketDefaultOwner(args[1], args[2]);
                        break;

                    case "remove-default-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        RemoveBucketDefaultOwner(args[1], args[2]);
                        break;

                    case "view-bucket-iam-members":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        ViewBucketIamMembers(args[1]);
                        break;

                    case "add-bucket-iam-member":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        AddBucketIamMember(args[1], args[2], args[3]);
                        break;

                    case "remove-bucket-iam-member":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        RemoveBucketIamMember(args[1], args[2], args[3]);
                        break;

                    case "generate-signed-url":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GenerateSignedUrl(args[1], args[2]);
                        break;

                    case "generate-encryption-key":
                        GenerateEncryptionKey();
                        break;

                    case "enable-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        EnableRequesterPays(args[1]);
                        break;

                    case "disable-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DisableRequesterPays(args[1]);
                        break;

                    case "get-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        return GetRequesterPays(args[1]) ? 1 : 0;

                    default:
                        PrintUsage();
                        return -1;
                }
                return 0;
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return null == e.Error ? -1 : e.Error.Code;
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
