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

using Google;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageTransfer.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Xunit;

namespace StorageTransfer.Samples.Tests
{
    [CollectionDefinition(nameof(StorageFixture))]
    public class StorageFixture : IDisposable, ICollectionFixture<StorageFixture>
    {
        public string ProjectId { get; }
        public string SourceAgentPoolName { get; }
        public string SinkAgentPoolName { get; }
        public StorageClient Storage { get; } = StorageClient.Create();
        public StorageTransferServiceClient Sts { get; } = StorageTransferServiceClient.Create();
        private readonly List<string> _bucketsToDelete = [];

        public StorageFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            SourceAgentPoolName = $"projects/{ProjectId}/agentPools/transfer_service_default";
            SinkAgentPoolName = $"projects/{ProjectId}/agentPools/transfer_service_default";
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
            }
        }

        internal void CreateBucketAndGrantStsPermissions(string bucketName, bool registerForDeletion = true)
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
            SleepAfterBucketCreateDelete();
            if (registerForDeletion)
            {
                RegisterBucketToDelete(bucketName);
            }
        }

        internal string GenerateBucketName() => Guid.NewGuid().ToString();
        internal string GetCurrentUserTempFolderPath() => System.IO.Path.GetTempPath();
        internal string GenerateTempFolderPath() => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        /// <summary>
        /// Bucket creation/deletion is rate-limited. To avoid making the tests flaky, we sleep after each operation.
        /// </summary>
        internal static void SleepAfterBucketCreateDelete() => Thread.Sleep(2000);

        internal void RegisterBucketToDelete(string bucket) => _bucketsToDelete.Add(bucket);

        public void Dispose()
        {
            foreach (var bucket in _bucketsToDelete)
            {
                DeleteBucket(Storage, bucket, null);
            }
        }

        private void DeleteBucket(StorageClient client, string bucket, string userProject)
        {
            try
            {
                client.DeleteBucket(bucket, new DeleteBucketOptions { UserProject = userProject, DeleteObjects = true });
            }
            catch (GoogleApiException)
            {
                // Some tests fail to delete buckets due to object retention locks etc.
                // They can be cleaned up later.
            }
            SleepAfterBucketCreateDelete();
        }
    }
}
