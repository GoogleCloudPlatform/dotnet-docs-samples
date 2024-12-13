/**
 * Copyright 2024 Google Inc.
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

using Google.Apis.Storage.v1.Data;
using Google.Cloud.PubSub.V1;
using Google.Cloud.Storage.V1;
using Google.Cloud.StorageTransfer.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace StorageTransfer.Samples.Tests
{
    [CollectionDefinition(nameof(StorageFixture))]
    public class StorageFixture : IDisposable, ICollectionFixture<StorageFixture>
    {
        public string ProjectId { get; }
        public string BucketNameSource { get; } = Guid.NewGuid().ToString();
        public string BucketNameSink { get; } = Guid.NewGuid().ToString();
        public string JobName { get; }
        public string SourceAgentPoolName { get; }
        public string SinkAgentPoolName { get; }
        public string GcsSourcePath { get; }
        public string RootDirectory { get; } = System.IO.Path.GetTempPath();
        public string DestinationDirectory { get; } = System.IO.Path.GetTempPath();
        public string TempDirectory { get; } = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        public string TempDestinationDirectory { get; } = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        public StorageClient Storage { get; } = StorageClient.Create();
        public string ManifestObjectName { get; } = "manifest.csv";
        public string TopicId { get; } = "DotNetTopic" + Guid.NewGuid().ToString();
        public string SubscriptionId { get; } = "DotNetSubscription" + Guid.NewGuid().ToString();
        public string PubSubId { get; }
        public StorageTransferServiceClient Sts { get; } = StorageTransferServiceClient.Create();

        public SubscriberServiceApiClient SubscriberClient { get; } = SubscriberServiceApiClient.Create();

        public PublisherServiceApiClient PublisherClient { get; } = PublisherServiceApiClient.Create();

        public StorageFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            SourceAgentPoolName = "projects/" + ProjectId + "/agentPools/transfer_service_default";
            SinkAgentPoolName = "projects/" + ProjectId + "/agentPools/transfer_service_default";
            PubSubId = "projects/" + ProjectId + "/subscriptions/" + SubscriptionId + "";
            GcsSourcePath = "foo/bar/";
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                throw new Exception("You need to set the Environment variable 'GOOGLE_PROJECT_ID' with your Google Cloud Project's project id.");
            }

            CreateBucketAndGrantStsPermissions(BucketNameSink);
            CreateBucketAndGrantStsPermissions(BucketNameSource);
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
            // Create subscription name
            SubscriptionName subscriptionName = new SubscriptionName(ProjectId, SubscriptionId);
            // Create topic name
            TopicName topicName = new TopicName(ProjectId, TopicId);
            // Create topic
            PublisherClient.CreateTopic(topicName);
            // Create subscription.
            SubscriberClient.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 500);
            var policyIamPolicyTopic = new Google.Cloud.Iam.V1.Policy();
            policyIamPolicyTopic.AddRoleMember("roles/pubsub.publisher", memberServiceAccount);
            PublisherClient.IAMPolicyClient.SetIamPolicy(new Google.Cloud.Iam.V1.SetIamPolicyRequest
            {
                ResourceAsResourceName = topicName,
                Policy = policyIamPolicyTopic
            });
            var policyIamPolicySubscriber = new Google.Cloud.Iam.V1.Policy();
            policyIamPolicySubscriber.AddRoleMember("roles/pubsub.subscriber", memberServiceAccount);
            PublisherClient.IAMPolicyClient.SetIamPolicy(new Google.Cloud.Iam.V1.SetIamPolicyRequest
            {
                ResourceAsResourceName = subscriptionName,
                Policy = policyIamPolicySubscriber
            });
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

        public void Dispose()
        {
            try
            {
                Storage.DeleteBucket(BucketNameSink);
            }
            catch (Exception)
            {
                // If bucket is not empty, we delete on a best effort basis.
                foreach (var storageObject in Storage.ListObjects(BucketNameSink, ""))
                {
                    Storage.DeleteObject(BucketNameSink, storageObject.Name);
                }
                Storage.DeleteBucket(BucketNameSink);
            }
            try
            {
                Storage.DeleteBucket(BucketNameSource);
            }
            catch (Exception)
            {
                // If bucket is not empty, we delete on a best effort basis.
                foreach (var storageObject in Storage.ListObjects(BucketNameSource, ""))
                {
                    Storage.DeleteObject(BucketNameSource, storageObject.Name);
                }
                Storage.DeleteBucket(BucketNameSource);
            }

            try
            {
                TopicName topicName = TopicName.FromProjectTopic(ProjectId, TopicId);
                PublisherClient.DeleteTopic(topicName);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Exception occur while deleting Topic {TopicId} Exception: {ex}");
            }

            try
            {
                SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(ProjectId, SubscriptionId);
                SubscriberClient.DeleteSubscription(subscriptionName);
            }
            catch (RpcException ex)
            {
                throw new Exception($"Exception occur while deleting subscription {SubscriptionId} Exception: {ex}");
            }

        }
    }
}
