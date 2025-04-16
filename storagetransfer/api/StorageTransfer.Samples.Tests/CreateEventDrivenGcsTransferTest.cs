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

using Google.Cloud.PubSub.V1;
using Google.Cloud.StorageTransfer.V1;
using System;
using Xunit;

namespace StorageTransfer.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class CreateEventDrivenGcsTransferTest : IDisposable
{
    private readonly StorageFixture _fixture;
    private readonly string _pubSubId;
    private string _transferJobName;
    private readonly string _sourceBucket;
    private readonly string _sinkBucket;
    private string TopicId { get; } = $"Topic-{Guid.NewGuid().ToString()}";
    private string SubscriptionId { get; } = $"Subscription-{Guid.NewGuid().ToString()}";
    private SubscriberServiceApiClient SubscriberClient { get; } = SubscriberServiceApiClient.Create();
    private PublisherServiceApiClient PublisherClient { get; } = PublisherServiceApiClient.Create();

    public CreateEventDrivenGcsTransferTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _sourceBucket = _fixture.GenerateBucketName();
        _sinkBucket = _fixture.GenerateBucketName();
        _fixture.CreateBucketAndGrantStsPermissions(_sourceBucket);
        _fixture.CreateBucketAndGrantStsPermissions(_sinkBucket);
        _pubSubId = $"projects/{_fixture.ProjectId}/subscriptions/{SubscriptionId}";
        CreatePubSubResourcesAndGrantStsPermissions();
    }

    [Fact]
    public void CreateEventDrivenGcsTransfer()
    {
        CreateEventDrivenGcsTransferSample createEventDrivenGcsTransferSample = new CreateEventDrivenGcsTransferSample();
        var transferJob = createEventDrivenGcsTransferSample.CreateEventDrivenGcsTransfer(_fixture.ProjectId, _sourceBucket, _sinkBucket, _pubSubId);
        Assert.Contains("transferJobs/", transferJob.Name);
        _transferJobName = transferJob.Name;
    }

    private void CreatePubSubResourcesAndGrantStsPermissions()
    {
        string email = _fixture.Sts.GetGoogleServiceAccount(new GetGoogleServiceAccountRequest()
        {
            ProjectId = _fixture.ProjectId
        }).AccountEmail;

        string memberServiceAccount = "serviceAccount:" + email;
        SubscriptionName subscriptionName = new SubscriptionName(_fixture.ProjectId, SubscriptionId);
        TopicName topicName = new TopicName(_fixture.ProjectId, TopicId);
        PublisherClient.CreateTopic(topicName);
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

    public void Dispose()
    {
        try
        {
            _fixture.Sts.UpdateTransferJob(new UpdateTransferJobRequest()
            {
                ProjectId = _fixture.ProjectId,
                JobName = _transferJobName,
                TransferJob = new TransferJob()
                {
                    Name = _transferJobName,
                    Status = TransferJob.Types.Status.Deleted
                }
            });

            TopicName topicName = TopicName.FromProjectTopic(_fixture.ProjectId, TopicId);
            PublisherClient.DeleteTopic(topicName);
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_fixture.ProjectId, SubscriptionId);
            SubscriberClient.DeleteSubscription(subscriptionName);
            _fixture.Storage.DeleteBucket(_sourceBucket);
            _fixture.Storage.DeleteBucket(_sinkBucket);
        }
        catch (Exception)
        {
            // Do nothing, we delete on a best effort basis.
        }
    }
}
