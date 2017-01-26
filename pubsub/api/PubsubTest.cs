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

using System;
// [START create_publisher_client]
using Google.Cloud.PubSub.V1;
// [END create_publisher_client]
using Google.Cloud.Iam.V1;
using System.Linq;
using Google.Protobuf;
using Grpc.Core;
using System.Collections.Generic;
using Xunit;
using Google.Api.Gax.Grpc;

namespace GoogleCloudSamples
{
    public class PubsubTest
    {
        private readonly string _projectId;
        private readonly PublisherClient _publisher;
        private readonly SubscriberClient _subscriber;
        // [START retry]
        /// <summary>
        /// Creates new CallSettings that will retry an RPC that fails.
        /// </summary>
        /// <param name="tryCount">
        /// How many times to try the RPC before giving up?
        /// </param>
        /// <param name="finalStatusCodes">
        /// Which status codes should we *not* retry?
        /// </param>
        /// <returns>
        /// A CallSettings instance.
        /// </returns>
        CallSettings newRetryCallSettings(int tryCount,
            params StatusCode[] finalStatusCodes)
        {
            // Initialize values for backoff settings to be used
            // by the CallSettings for RPC retries
            TimeSpan delay = TimeSpan.FromMilliseconds(500);
            TimeSpan maxDelay = TimeSpan.FromSeconds(3);
            double delayMultiplier = 2;
            var backoff = new BackoffSettings(delay, maxDelay, delayMultiplier);

            return new CallSettings(null, null,
                CallTiming.FromRetry(new RetrySettings(backoff, backoff,
                Google.Api.Gax.Expiration.None,
                  (RpcException e) => (
                        StatusCode.OK != e.Status.StatusCode
                        && !finalStatusCodes.Contains(e.Status.StatusCode)
                        && --tryCount > 0),
                    RetrySettings.NoJitter)),
                metadata => metadata.Add("ClientVersion", "1.0.0"), null, null);
        }
        // [END retry]

        public PubsubTest()
        {
            // [START create_publisher_client]
            // By default, the Google.Pubsub.V1 library client will authenticate 
            // using the service account file (created in the Google Developers 
            // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS 
            // environment variable and it will use the project specified by 
            // the GOOGLE_PROJECT_ID environment variable. If you are running on
            // a Google Compute Engine VM, authentication is completely 
            // automatic.
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            // [END create_publisher_client]
            _publisher = CreatePublisherClient();
            _subscriber = CreateSubscriberClient();
        }

        public PublisherClient CreatePublisherClient()
        {
            // [START create_publisher_client]
            PublisherClient publisher = PublisherClient.Create();
            // [END create_publisher_client]
            return publisher;
        }

        public SubscriberClient CreateSubscriberClient()
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            return subscriber;
        }

        public void CreateTopic(string topicId, PublisherClient publisher)
        {
            // [START create_topic]
            TopicName topicName = new TopicName(_projectId, topicId);
            try
            {
                publisher.CreateTopic(topicName);
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }
            // [END create_topic]
        }

        public void CreateSubscription(string topicId, string subscriptionId,
            SubscriberClient subscriber)
        {
            // [START create_subscription]
            TopicName topicName = new TopicName(_projectId, topicId);
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            try
            {
                Subscription subscription = subscriber.CreateSubscription(
                    subscriptionName, topicName, pushConfig: null,
                    ackDeadlineSeconds: 60);
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }
            // [END create_subscription]
        }

        public void CreateTopicMessage(string topicId, PublisherClient publisher)
        {
            // [START publish_message]
            TopicName topicName = new TopicName(_projectId, topicId);
            PubsubMessage message = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8("Hello Cloud Pub/Sub!"),
                // The attributes provide metadata in a string-to-string 
                // dictionary.
                Attributes =
                {
                    { "description", "Simple text message" }
                }
            };
            publisher.Publish(topicName, new[] { message });
            // [END publish_message]
        }

        public PullResponse PullTopicMessage(string subscriptionId,
            SubscriberClient subscriber)
        {
            // [START pull_messages]
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            PullResponse response = subscriber.Pull(subscriptionName,
                returnImmediately: true, maxMessages: 10);
            // [END pull_messages]
            return response;
        }

        public void AcknowledgeTopicMessage(
            string subscriptionId,
            SubscriberClient subscriber,
            PullResponse response)
        {
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            // [START pull_messages]
            subscriber.Acknowledge(subscriptionName,
                response.ReceivedMessages.Select(m => m.AckId));
            // [END pull_messages]
        }

        public Topic GetTopic(string topicId, PublisherClient publisher)
        {
            TopicName topicName = new TopicName(_projectId, topicId);
            Topic topic = _publisher.GetTopic(topicName);
            return topic;
        }

        public Subscription GetSubscription(string subscriptionId,
            SubscriberClient subscriber)
        {
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            Subscription subscription = _subscriber.GetSubscription(
                subscriptionName);
            return subscription;
        }

        public Policy GetTopicIamPolicy(string topicId, PublisherClient publisher)
        {
            // [START pubsub_get_topic_policy]
            TopicName topicName = new TopicName(_projectId, topicId);
            Policy policy = _publisher.GetIamPolicy(topicName.ToString());
            return policy;
            // [END pubsub_get_topic_policy]
        }

        public Policy GetSubscriptionIamPolicy(string subscriptionId, PublisherClient publisher)
        {
            // [START pubsub_get_subscription_policy]
            SubscriptionName subscriptionName = new SubscriptionName(_projectId, subscriptionId);
            Policy policy = _publisher.GetIamPolicy(subscriptionName.ToString());
            return policy;
            // [END pubsub_get_subscription_policy]
        }

        public Policy SetTopicIamPolicy(string topicId, PublisherClient publisher)
        {
            // [START pubsub_set_topic_policy]
            Policy policy = new Policy
            {
                Bindings =
                    {
                        new Binding { Role = "roles/pubsub.editor",
                            Members = { "group:cloud-logs@google.com" } },
                        new Binding { Role = "roles/pubsub.viewer",
                            Members = { "allUsers"} }
                    }
            };

            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = new TopicName(_projectId, topicId).ToString(),
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            return response;
            // [END pubsub_set_topic_policy]
        }

        public Policy SetSubscriptionIamPolicy(string subscriptionId, PublisherClient publisher)
        {
            // [START pubsub_set_subscription_policy]
            Policy policy = new Policy
            {
                Bindings =
                {
                    new Binding { Role = "roles/pubsub.editor",
                        Members = { "group:cloud-logs@google.com" } },
                    new Binding { Role = "roles/pubsub.viewer",
                        Members = { "allUsers" } }
                }
            };
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = new SubscriptionName(_projectId, subscriptionId).ToString(),
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            return response;
            // [END pubsub_set_subscription_policy]
        }

        public TestIamPermissionsResponse TestTopicIamPermissionsResponse(string topicId,
            PublisherClient publisher)
        {
            // [START pubsub_test_topic_permissons]
            List<string> permissions = new List<string>();
            permissions.Add("pubsub.topics.get");
            permissions.Add("pubsub.topics.update");
            TestIamPermissionsRequest request = new TestIamPermissionsRequest
            {
                Resource = new TopicName(_projectId, topicId).ToString(),
                Permissions = { permissions }
            };
            TestIamPermissionsResponse response = publisher.TestIamPermissions(request);
            return response;
            // [END pubsub_test_topic_permissons]
        }

        public TestIamPermissionsResponse TestSubscriptionIamPermissionsResponse(
            string subscriptionId, PublisherClient publisher)
        {
            // [START pubsub_test_subscription_permissons]
            List<string> permissions = new List<string>();
            permissions.Add("pubsub.subscriptions.get");
            permissions.Add("pubsub.subscriptions.update");
            TestIamPermissionsRequest request = new TestIamPermissionsRequest
            {
                Resource = new TopicName(_projectId, subscriptionId).ToString(),
                Permissions = { permissions }
            };
            TestIamPermissionsResponse response = publisher.TestIamPermissions(request);
            return response;
            // [END pubsub_test_subscription_permissons]
        }

        public IEnumerable<Topic> ListProjectTopics(PublisherClient publisher)
        {
            // [START list_topics]
            ProjectName projectName = new ProjectName(_projectId);
            IEnumerable<Topic> topics = publisher.ListTopics(projectName);
            // [END list_topics]
            return topics;
        }

        public IEnumerable<Subscription> ListSubscriptions(SubscriberClient
            subscriber)
        {
            // [START list_subscriptions]
            ProjectName projectName = new ProjectName(_projectId);
            IEnumerable<Subscription> subscriptions =
                subscriber.ListSubscriptions(projectName);
            // [END list_subscriptions]
            return subscriptions;
        }

        public void DeleteSubscription(string subscriptionId, SubscriberClient
            subscriber)
        {
            // [START delete_subscription]
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            subscriber.DeleteSubscription(subscriptionName);
            // [END delete_subscription]
        }

        public void DeleteTopic(string topicId, PublisherClient publisher)
        {
            // [START delete_topic]
            TopicName topicName = new TopicName(_projectId, topicId);
            publisher.DeleteTopic(topicName);
            // [END delete_topic]
        }

        // [START retry]
        public void RpcRetry(string topicId, string subscriptionId,
            PublisherClient publisher, SubscriberClient subscriber)
        {
            TopicName topicName = new TopicName(_projectId, topicId);
            // Create Subscription.
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            // Create Topic
            try
            {
                // This may fail if the Topic already exists.
                // Don't retry in that case.
                publisher.CreateTopic(topicName, newRetryCallSettings(3,
                    StatusCode.AlreadyExists));
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }
            try
            {
                // Subscribe to Topic
                // This may fail if the Subscription already exists or 
                // the Topic has not yet been created.  In those cases, don't
                // retry, because a retry would fail the same way.
                subscriber.CreateSubscription(subscriptionName, topicName,
                    pushConfig: null, ackDeadlineSeconds: 60,
                    callSettings: newRetryCallSettings(3, StatusCode.AlreadyExists,
                        StatusCode.NotFound));
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  That's fine.
            }
        }
        // [END retry]

        private static bool IsEmptyResponse(PullResponse response)
        {
            foreach (var result in response.ReceivedMessages)
                return false;
            return true;
        }

        [Fact]
        public void TestCreateTopic()
        {
            string topicId = "testTopicForTopicCreation";
            CreateTopic(topicId, _publisher);
            TopicName topicName = new TopicName(_projectId, topicId);
            Topic topic = GetTopic(topicId, _publisher);
            Assert.Equal(topicName.ToString(), topic.Name);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestCreateSubscription()
        {
            string topicId = "testTopicForSubscriptionCreation";
            string subscriptionId = "testSubscriptionForSubscriptionCreation";
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            Subscription subscription = GetSubscription(subscriptionId,
                _subscriber);
            Assert.Equal(subscriptionName.ToString(), subscription.Name);
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestCreateTopicMessage()
        {
            string topicId = "testTopicForMessageCreation";
            string subscriptionId = "testSubscriptionForMessageCreation";
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            CreateTopicMessage(topicId, _publisher);
            //Pull the Message to confirm it is valid
            PullResponse response = PullTopicMessage(subscriptionId, _subscriber);
            Assert.False(IsEmptyResponse(response));
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestAcknowledgeTopicMessage()
        {
            string topicId = "testTopicForMessageAcknowledgement";
            string subscriptionId = "testSubscriptionForMessageAcknowledgement";
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            CreateTopicMessage(topicId, _publisher);
            //Pull the Message
            PullResponse response = PullTopicMessage(subscriptionId, _subscriber);
            //Acknowledge the Message
            AcknowledgeTopicMessage(subscriptionId, _subscriber, response);
            //Pull the Message to confirm it is gone after it is acknowledged
            response = PullTopicMessage(subscriptionId, _subscriber);
            Assert.True(IsEmptyResponse(response));
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestListTopics()
        {
            string topicId = "testTopicForListingTopics";
            TopicName topicName = new TopicName(_projectId, topicId);
            CreateTopic(topicId, _publisher);
            IEnumerable<Topic> topics = ListProjectTopics(_publisher);
            Assert.False(topics.Count() == 0);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestListSubscriptions()
        {
            string topicId = "testTopicForListingSubscriptions";
            string subscriptionId = "testSubscriptionForListingSubscriptions";
            TopicName topicName = new TopicName(_projectId, topicId);
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            IEnumerable<Subscription> subscriptions = ListSubscriptions(
                _subscriber);
            Assert.False(subscriptions.Count() == 0);
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestGetTopicIamPolicy()
        {
            string topicId = "testTopicForGetTopicIamPolicy";
            CreateTopic(topicId, _publisher);
            Policy policy = GetTopicIamPolicy(topicId, _publisher);
            Assert.NotEmpty(policy.ToString());
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestGetSubscriptionIamPolicy()
        {
            string topicId = "testTopicForGetSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionForGetSubscriptionIamPolicy";
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            Policy policy = GetSubscriptionIamPolicy(subscriptionId, _publisher);
            Assert.NotEmpty(policy.ToString());
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestSetTopicIamPolicy()
        {
            string topicId = "testTopicForSetTopicIamPolicy";
            string testMemberValueToConfirm = "cloud-logs@google.com";
            string testRoleValueToConfirm = "roles/pubsub.editor";
            CreateTopic(topicId, _publisher);
            SetTopicIamPolicy(topicId, _publisher);
            Policy policy = GetTopicIamPolicy(topicId, _publisher);
            Assert.Contains(testRoleValueToConfirm, policy.Bindings.First().Role.ToString());
            Assert.Contains(testMemberValueToConfirm,
                policy.Bindings.First().Members.First().ToString());
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestSetSubscriptionIamPolicy()
        {
            string topicId = "testTopicForSetSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionForSetSubscriptionIamPolicy";
            string testMemberValueToConfirm = "cloud-logs@google.com";
            string testRoleValueToConfirm = "roles/pubsub.editor";
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            SetSubscriptionIamPolicy(subscriptionId, _publisher);
            Policy policy = GetSubscriptionIamPolicy(subscriptionId, _publisher);
            Assert.Contains(testRoleValueToConfirm, policy.Bindings.First().Role.ToString());
            Assert.Contains(testMemberValueToConfirm,
                policy.Bindings.First().Members.First().ToString());
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestTopicIamPolicyPermissions()
        {
            string topicId = "testTopicForTestTopicIamPolicy";
            CreateTopic(topicId, _publisher);
            SetTopicIamPolicy(topicId, _publisher);
            TestIamPermissionsResponse response =
                TestTopicIamPermissionsResponse(topicId, _publisher);
            Assert.NotEmpty(response.ToString());
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestSubscriptionIamPolicyPermissions()
        {
            string topicId = "testTopicForTestSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionForTestSubscriptionIamPolicy";
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            SetSubscriptionIamPolicy(subscriptionId, _publisher);
            TestIamPermissionsResponse response =
                TestSubscriptionIamPermissionsResponse(subscriptionId, _publisher);
            Assert.NotEmpty(response.ToString());
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestRpcRetry()
        {
            string topicId = "testTopicForRpcRetry";
            string subscriptionId = "testSubscriptionForRpcRetry";
            RpcRetry(topicId, subscriptionId, _publisher,
                _subscriber);
            TopicName topicName = new TopicName(_projectId, topicId);
            Topic topic = GetTopic(topicId, _publisher);
            Assert.Equal(topicName.ToString(), topic.Name);
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestDeleteTopic()
        {
            string topicId = "testTopicForDeleteTopic";
            TopicName topicName = new TopicName(_projectId, topicId);
            CreateTopic(topicId, _publisher);
            DeleteTopic(topicId, _publisher);
            Exception ex = Assert.Throws<Grpc.Core.RpcException>(() =>
                _publisher.GetTopic(topicName));
        }

        [Fact]
        public void TestDeleteSubscription()
        {
            string topicId = "testTopicForDeleteSubscription";
            string subscriptionId = "testSubscriptionForDeleteSubscription";
            TopicName topicName = new TopicName(_projectId, topicId);
            SubscriptionName subscriptionName = new SubscriptionName(_projectId,
                subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            DeleteSubscription(subscriptionId, _subscriber);
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                _subscriber.GetSubscription(subscriptionName));
            DeleteTopic(topicId, _publisher);
        }
    }
}
