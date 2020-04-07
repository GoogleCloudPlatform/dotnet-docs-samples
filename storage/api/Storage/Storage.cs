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

using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GoogleCloudSamples
{
    public class Storage
    {
        public static readonly string s_projectId = "YOUR-PROJECT-ID";

        private static readonly string s_usage =
            "Usage: \n" +
            "  Storage create [new-bucket-name]\n" +
            "  Storage create-regional-bucket location [new-bucket-name]\n" +
            "  Storage list\n" +
            "  Storage list bucket-name [prefix] [delimiter]\n" +
            "  Storage get-metadata bucket-name object-name\n" +
            "  Storage get-bucket-metadata bucket-name\n" +
            "  Storage make-public bucket-name object-name\n" +
            "  Storage upload [-key encryption-key] bucket-name local-file-path [object-name]\n" +
            "  Storage copy source-bucket-name source-object-name dest-bucket-name dest-object-name\n" +
            "  Storage move bucket-name source-object-name dest-object-name\n" +
            "  Storage download [-key encryption-key] bucket-name object-name [local-file-path]\n" +
            "  Storage download-byte-range bucket-name object-name range-begin range-end [local-file-path]\n" +
            "  Storage generate-signed-url bucket-name object-name\n" +
            "  Storage generate-signed-get-url-v4 bucket-name object-name\n" +
            "  Storage generate-signed-put-url-v4 bucket-name object-name\n" +
            "  Storage view-bucket-iam-members bucket-name\n" +
            "  Storage add-bucket-iam-member bucket-name role member\n" +
            "  Storage add-bucket-iam-conditional-binding bucket-name member\n" +
            "                              role member cond-title cond-description cond-expression\n" +
            "  Storage remove-bucket-iam-member bucket-name role member\n" +
            "  Storage remove-bucket-iam-conditional-binding bucket-name role\n" +
            "                               cond-title cond-description cond-expression\n" +
            "  Storage add-bucket-default-kms-key bucket-name key-location key-ring key-name\n" +
            "  Storage upload-with-kms-key bucket-name key-location\n" +
            "                              key-ring key-name local-file-path [object-name]\n" +
            "  Storage print-acl bucket-name\n" +
            "  Storage print-acl bucket-name object-name\n" +
            "  Storage create-hmac-key service-account-email\n" +
            "  Storage get-hmac-key access-id\n" +
            "  Storage list-hmac-keys\n" +
            "  Storage deactivate-hmac-key access-id\n" +
            "  Storage activate-hmac-key access-id\n" +
            "  Storage delete-hmac-key access-id\n" +
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
            "  Storage generate-encryption-key\n" +
            "  Storage get-bucket-default-event-based-hold bucket-name\n" +
            "  Storage enable-bucket-default-event-based-hold bucket-name\n" +
            "  Storage disable-bucket-default-event-based-hold bucket-name\n" +
            "  Storage lock-bucket-retention-policy bucket-name\n" +
            "  Storage set-bucket-retention-policy bucket-name retention-period\n" +
            "  Storage remove-bucket-retention-policy bucket-name\n" +
            "  Storage get-bucket-retention-policy bucket-name\n" +
            "  Storage set-object-temporary-hold bucket-name object-name\n" +
            "  Storage release-object-temporary-hold bucket-name object-name\n" +
            "  Storage set-object-event-based-hold bucket-name object-name\n" +
            "  Storage release-object-event-based-hold bucket-name object-name\n" +
            "  Storage enable-uniform-bucket-level-access bucket-name\n" +
            "  Storage disable-uniform-bucket-level-access bucket-name\n" +
            "  Storage get-uniform-bucket-level-access bucket-name\n";

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

        private string PullFlag(string flag, ref string[] args, bool requiresValue)
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
                Console.WriteLine("Update Storage.cs and replace YOUR-PROJECT" +
                    "-ID with your project id, and recompile.");
                return -1;
            }
            if (args.Length < 1 && PrintUsage()) return -1;
            try
            {
                switch (args[0].ToLower())
                {
                    case "create":
                        CreateBucket.StorageCreateBucket(s_projectId, args.Length < 2 ? RandomBucketName() : args[1]);
                        break;

                    case "create-regional-bucket":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        CreateRegionalBucket.StorageCreateRegionalBucket(s_projectId, args[1], args.Length < 3 ? RandomBucketName() : args[2]);
                        break;

                    case "list":
                        if (args.Length < 2)
                            ListBuckets.GetBucketList(s_projectId);
                        else if (args.Length < 3)
                            ListFiles.GetFileList(args[1]);
                        else
                            ListFilesWithPrefix.GetFileListWithPrefix(args[1], args[2],
                                args.Length < 4 ? null : args[3]);
                        break;

                    case "delete":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        if (args.Length < 3)
                        {
                            DeleteBucket.StorageDeleteBucket(args[1]);
                        }
                        else
                        {
                            DeleteFile.StorageDeleteFile(args[1], args.Skip(2));
                        }
                        break;

                    case "upload":
                        encryptionKey = PullFlag("-key", ref args, requiresValue: true);
                        requesterPays = PullFlag("-pay", ref args, requiresValue: false);
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (encryptionKey != null)
                        {
                            UploadEncryptedFile.StorageUploadEncryptedFile(encryptionKey, args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else if (requesterPays != null)
                        {
                            UploadFileRequesterPays.StorageUploadFileRequesterPays(s_projectId, args[1],
                                args[2], args.Length < 4 ? null : args[3]);
                        }
                        else
                        {
                            UploadFile.StorageUploadFile(args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        break;

                    case "upload-with-kms-key":
                        if (args.Length < 6 && PrintUsage()) return -1;
                        UploadFileWithKmsKey.UploadEncryptedFileWithKmsKey(s_projectId, args[1], args[2], args[3],
                            args[4], args[5], args.Length < 7 ? null : args[6]);
                        break;

                    case "download":
                        encryptionKey = PullFlag("-key", ref args, requiresValue: true);
                        requesterPays = PullFlag("-pay", ref args, requiresValue: false);
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (encryptionKey != null)
                        {
                            DownloadEncryptedFile.StorageDownloadEncryptedFile(encryptionKey, args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else if (requesterPays != null)
                        {
                            DownloadObjectRequesterPays.StorageDownloadObjectRequesterPays(s_projectId,
                                args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        else
                        {
                            DownloadFile.StorageDownloadFile(args[1], args[2], args.Length < 4 ? null : args[3]);
                        }
                        break;

                    case "download-byte-range":
                        if (args.Length < 5 && PrintUsage()) return -1;
                        DownloadByteRange.StorageDownloadByteRange(args[1], args[2],
                            long.Parse(args[3]), long.Parse(args[4]),
                            args.Length < 6 ? null : args[4]);
                        break;

                    case "get-metadata":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GetMetadata.StorageGetMetadata(args[1], args[2]);
                        break;

                    case "get-bucket-metadata":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        GetBucketMetadata.StorageGetBucketMetadata(args[1]);
                        break;

                    case "make-public":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        MakePublic.StorageMakePublic(args[1], args[2]);
                        break;

                    case "move":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        MoveFile.MoveObject(args[1], args[2], args[3]);
                        break;

                    case "copy":
                        if (args.Length < 5 && PrintUsage()) return -1;
                        CopyFile.CopyObject(args[1], args[2], args[3], args[4]);
                        break;

                    case "print-acl":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        if (args.Length < 3)
                            PrintBucketAcl.StoragePrintBucketAcl(args[1]);
                        else
                            PrintFileAcl.PrintObjectAcl(args[1], args[2]);
                        break;

                    case "print-acl-for-user":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            PrintBucketAclForUser.StoragePrintBucketAclForUser(args[1], args[2]);
                        else
                            PrintFileAclForUser.PrintObjectAclForUser(args[1], args[2], args[3]);
                        break;

                    case "print-default-acl":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        PrintBucketDefaultAcl.StoragePrintBucketDefaultAcl(args[1]);
                        break;

                    case "create-hmac-key":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        HmacKeyCreate.CreateHmacKey(s_projectId, args[1]);
                        break;

                    case "list-hmac-keys":
                        HmacKeysList.ListHmacKeys(s_projectId);
                        break;

                    case "get-hmac-key":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        HmacKeyGet.GetHmacKey(s_projectId, args[1]);
                        break;

                    case "deactivate-hmac-key":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        HmacKeyDeactivate.DeactivateHmacKey(s_projectId, args[1]);
                        break;

                    case "activate-hmac-key":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        HmacKeyActivate.ActivateHmacKey(s_projectId, args[1]);
                        break;

                    case "delete-hmac-key":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        HmacKeyDelete.DeleteHmacKey(s_projectId, args[1]);
                        break;

                    case "add-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            AddBucketOwner.StorageAddBucketOwner(args[1], args[2]);
                        else
                            AddFileOwner.AddObjectOwner(args[1], args[2], args[3]);
                        break;

                    case "remove-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        if (args.Length < 4)
                            RemoveBucketOwner.StorageRemoveBucketOwner(args[1], args[2]);
                        else
                            RemoveFileOwner.RemoveObjectOwner(args[1], args[2], args[3]);
                        break;

                    case "add-default-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        AddBucketDefaultOwner.StorageAddBucketDefaultOwner(args[1], args[2]);
                        break;

                    case "remove-default-owner":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        RemoveBucketDefaultOwner.StorageRemoveBucketDefaultOwner(args[1], args[2]);
                        break;

                    case "view-bucket-iam-members":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        StorageViewBucketIamMembers.ViewBucketIamMembers(args[1]);
                        break;

                    case "add-bucket-iam-member":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        AddBucketIamMember.StorageAddBucketIamMember(args[1], args[2], args[3]);
                        break;

                    case "add-bucket-iam-conditional-binding":
                        if (args.Length < 7 && PrintUsage()) return -1;
                        AddBucketConditionalIamBinding.StorageAddBucketConditionalIamBinding(args[1], args[2], args[3], args[4], args[5], args[6]);
                        break;

                    case "remove-bucket-iam-conditional-binding":
                        if (args.Length < 6 && PrintUsage()) return -1;
                        RemoveBucketConditionalIamBinding.StorageRemoveBucketConditionalIamBinding(args[1], args[2], args[3], args[4], args[5]);
                        break;

                    case "remove-bucket-iam-member":
                        if (args.Length < 4 && PrintUsage()) return -1;
                        RemoveBucketIamMember.StorageRemoveBucketIamMember(args[1], args[2], args[3]);
                        break;

                    case "add-bucket-default-kms-key":
                        if (args.Length < 5 && PrintUsage()) return -1;
                        EnableDefaultKMSKey.AddBucketDefaultKmsKey(s_projectId, args[1], args[2], args[3], args[4]);
                        break;

                    case "generate-signed-url":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GenerateSignedUrl.StorageGenerateSignedUrl(args[1], args[2]);
                        break;

                    case "generate-signed-get-url-v4":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GenerateV4ReadSignedUrl.StorageGenerateV4SignedReadUrl(args[1], args[2]);
                        break;

                    case "generate-signed-put-url-v4":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        GenerateV4UploadSignedUrl.GenerateV4SignedPutUrl(args[1], args[2]);
                        break;

                    case "generate-encryption-key":
                        GenerateEncryptionKey.StorageGenerateEncryptionKey();
                        break;

                    case "enable-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        EnableRequesterPays.StorageEnableRequesterPays(s_projectId, args[1]);
                        break;

                    case "disable-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DisableRequesterPays.StorageDisableRequesterPays(s_projectId, args[1]);
                        break;

                    case "get-requester-pays":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        return GetRequesterPaysStatus.GetRequesterPays(s_projectId, args[1]) ? 1 : 0;

                    case "get-bucket-default-event-based-hold":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        return GettDefaultEventBasedHold.GetBucketDefaultEventBasedHold(args[1]) ? 1 : 0;

                    case "enable-bucket-default-event-based-hold":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        EnableBucketDefaultEventBasedHold.StorageEnableBucketDefaultEventBasedHold(args[1]);
                        break;

                    case "disable-bucket-default-event-based-hold":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DisableDefaultEventBasedHold.DisableBucketDefaultEventBasedHold(args[1]);
                        break;

                    case "lock-bucket-retention-policy":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        LockRetentionPolicy.LockBucketRetentionPolicy(args[1]);
                        break;

                    case "set-bucket-retention-policy":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        SetRetentionPolicy.SetBucketRetentionPolicy(args[1], long.Parse(args[2]));
                        break;

                    case "remove-bucket-retention-policy":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        RemoveRetentionPolicy.RemoveBucketRetentionPolicy(args[1]);
                        break;

                    case "get-bucket-retention-policy":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        GetRetentionPolicy.GetBucketRetentionPolicy(args[1]);
                        break;

                    case "set-object-temporary-hold":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        SetTemporarydHold.SetObjectTemporaryHold(args[1], args[2]);
                        break;

                    case "release-object-temporary-hold":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        ReleaseTemporaryHold.ReleaseObjectTemporaryHold(args[1], args[2]);
                        break;

                    case "set-object-event-based-hold":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        SetEventBasedHold.SetObjectEventBasedHold(args[1], args[2]);
                        break;

                    case "release-object-event-based-hold":
                        if (args.Length < 3 && PrintUsage()) return -1;
                        ReleaseEventBasedHold.ReleaseObjectEventBasedHold(args[1], args[2]);
                        break;

                    case "enable-uniform-bucket-level-access":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        EnableUniformBucketLevelAccess.StorageEnableUniformBucketLevelAccess(args[1]);
                        break;

                    case "disable-uniform-bucket-level-access":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        DisableUniformBucketLevelAccess.StorageDisableUniformBucketLevelAccess(args[1]);
                        break;

                    case "get-uniform-bucket-level-access":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        GetUniformBucketLevelAccess.StorageGetUniformBucketLevelAccess(args[1]);
                        break;

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
