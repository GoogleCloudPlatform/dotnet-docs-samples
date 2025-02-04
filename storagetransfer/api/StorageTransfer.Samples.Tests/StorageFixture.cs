/**
 * Copyright 2021 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageTransfer.V1;
using Xunit;

namespace StorageTransfer.Samples.Tests
{
    [CollectionDefinition(nameof(StorageFixture))]
    public class StorageFixture : IDisposable, ICollectionFixture<StorageFixture>
    {
        public string ProjectId { get; }
        public string BucketNameSource { get; } = Guid.NewGuid().ToString();
        public string BucketNameSink { get; } = Guid.NewGuid().ToString();
        public string BucketNameManifestSource { get; } = Guid.NewGuid().ToString();
        public string BucketNamePosixSource { get; } = Guid.NewGuid().ToString();
        public string JobName { get; }
        public string SourceAgentPoolName { get; }
        public string SinkAgentPoolName { get; }
        public string GcsSourcePath { get; }
        public StorageClient Storage { get; } = StorageClient.Create();
        public string ManifestObjectName { get; } = "manifest.csv";
        public StorageTransferServiceClient Sts { get; } = StorageTransferServiceClient.Create();

        public StorageFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            SourceAgentPoolName = $"projects/{ProjectId}/agentPools/transfer_service_default";
            SinkAgentPoolName = $"projects/{ProjectId}/agentPools/transfer_service_default";
            GcsSourcePath = "foo/bar/";
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
            }

            CreateBucketAndGrantStsPermissions(BucketNameSink);
            CreateBucketAndGrantStsPermissions(BucketNameSource);
            CreateBucketAndGrantStsPermissions(BucketNameManifestSource);
            CreateBucketAndGrantStsPermissions(BucketNamePosixSource);
            UploadObjectToManifestBucket(BucketNameManifestSource);
            UploadObjectToPosixBucket(BucketNamePosixSource);
            // Initialize request argument(s)
            TransferJob transferJob = new TransferJob
            {
                ProjectId = ProjectId,
                TransferSpec = new TransferSpec
                {
                    GcsDataSink = new GcsData { BucketName = BucketNameSource },
                    GcsDataSource = new GcsData { BucketName = BucketNameSink }
                },
                Status = TransferJob.Types.Status.Enabled
            };
            CreateTransferJobRequest request = new CreateTransferJobRequest
            {
                TransferJob = transferJob
            };
            // Make the request
            TransferJob response = Sts.CreateTransferJob(new CreateTransferJobRequest { TransferJob = transferJob });
            JobName = response.Name;
            string email = Sts.GetGoogleServiceAccount(new GetGoogleServiceAccountRequest()
            {
                ProjectId = ProjectId
            }).AccountEmail;
            string memberServiceAccount = "serviceAccount:" + email;
        }

        private void CreateBucketAndGrantStsPermissions(string bucketName)
        {
            var bucket = Storage.CreateBucket(ProjectId, new Bucket
            {
                Name = bucketName
            });

            string email = Sts.GetGoogleServiceAccount(new GetGoogleServiceAccountRequest()
            {
                ProjectId = ProjectId
            }).AccountEmail;
            string member = "serviceAccount:" + email;
            string objectViewer = "roles/storage.objectViewer";
            string bucketReader = "roles/storage.legacyBucketReader";
            string bucketWriter = "roles/storage.legacyBucketWriter";

            var policy = Storage.GetBucketIamPolicy(bucketName, new GetBucketIamPolicyOptions
            {
                RequestedPolicyVersion = 3
            });
            // Set the policy schema version. For more information, please refer to https://cloud.google.com/iam/docs/policies#versions.
            policy.Version = 3;

            Policy.BindingsData objectViewerBinding = new Policy.BindingsData
            {
                Role = objectViewer,
                Members = new List<string> { member }
            };
            Policy.BindingsData bucketReaderBinding = new Policy.BindingsData
            {
                Role = bucketReader,
                Members = new List<string> { member }
            };
            Policy.BindingsData bucketWriterBinding = new Policy.BindingsData
            {
                Role = bucketWriter,
                Members = new List<string> { member }
            };
            policy.Bindings.Add(objectViewerBinding);
            policy.Bindings.Add(bucketReaderBinding);
            policy.Bindings.Add(bucketWriterBinding);
            Storage.SetBucketIamPolicy(bucketName, policy);
        }

        private void UploadObjectToManifestBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("flower.jpeg");
            MemoryStream stream = new MemoryStream(byteArray);
            storage.UploadObject(bucketName, ManifestObjectName, "application/octet-stream", stream);
        }

        private void UploadObjectToPosixBucket(string bucketName)
        {
            var storage = StorageClient.Create();
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("flower.jpeg");
            MemoryStream stream = new MemoryStream(byteArray);
            string fileName = $"{GcsSourcePath}{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            storage.UploadObject(bucketName, fileName, "application/octet-stream", stream);
        }
        internal string GetCurrentUserTempFolderPath() => System.IO.Path.GetTempPath();
        internal string GenerateTempFolderPath() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        public void Dispose()
        {
            try
            {
                Storage.DeleteBucket(BucketNameSink, new DeleteBucketOptions { DeleteObjects = true });
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
            try
            {
                Storage.DeleteBucket(BucketNameSource, new DeleteBucketOptions { DeleteObjects = true });
            }
            catch (Exception)
            {
                // Do nothing, we delete on a best effort basis.
            }
        }
    }
}
