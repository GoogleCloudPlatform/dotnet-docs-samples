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
using System.Threading.Tasks;
using System.Threading;

namespace GoogleCloudSamples
{
    public class PubsubTest : IDisposable
    {
        private readonly string _projectId;
        private readonly PublisherClient _publisher;
        private readonly SubscriberClient _subscriber;
        private readonly List<string> _tempTopicIds = new List<string>();
        private readonly List<string> _tempSubscriptionIds = new List<string>();

        readonly CommandLineRunner _pubsub = new CommandLineRunner()
        {
            VoidMain = Pubsub.Main,
            Command = "Pubsub"
        };

        void IDisposable.Dispose()
        {
            foreach (string topicId in _tempTopicIds)
            {
                Run("deleteTopic", _projectId, topicId);
            }
            foreach (string subscriptionId in _tempSubscriptionIds)
            {
                Run("deleteSubscription", _projectId, subscriptionId);
            }
        }

        protected ConsoleOutput Run(params string[] args)
        {
            string cmd = args[0].ToLower();
            var output = _pubsub.Run(args);
            switch (cmd)
            {
                case "createtopic":
                    _tempTopicIds.Add(args[2]);
                    break;
                case "createsubscription":
                    _tempSubscriptionIds.Add(args[3]);
                    break;
            }
            return output;
        }

        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };

        void Eventually(Action action) => _retryRobot.Eventually(action);

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

        // [START retry]
        internal void RpcRetry(string topicId, string subscriptionId,
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

        [Fact]
        public void TestCreateTopic()
        {
            string topicId = "testTopicForTopicCreation";
            var output = Run("createTopic", _projectId, topicId);
            var topicDetails = Run("getTopic", _projectId, topicId);
            Assert.Contains($"{topicId}", topicDetails.Stdout);
        }

        [Fact]
        public void TestCreateSubscription()
        {
            string topicId = "testTopicForSubscriptionCreation";
            string subscriptionId = "testSubscriptionForSubscriptionCreation";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            var subscriptionDetails = Run("getSubscription", _projectId, subscriptionId);
            Assert.Contains($"{subscriptionId}", subscriptionDetails.Stdout);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestPublishMessage(bool customBatch)
        {
            string topicId = "testTopicForMessageCreation";
            string subscriptionId = "testSubscriptionForMessageCreation";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            List<string> cmdArgs = new[] { "publishMessages", _projectId,
                topicId, "Hello World!", "Good day.", "Bye bye." }.ToList();
            if (customBatch)
            {
                cmdArgs.Insert(2, "-b");
            }
            var messageCreateOutput = Run(cmdArgs.ToArray());
            Assert.Equal(0, messageCreateOutput.ExitCode);
            Assert.Contains("Published message", messageCreateOutput.Stdout);
            // Pull the Message to confirm it is valid
            Eventually(() =>
            {
                var output = Run("pullMessages", _projectId, subscriptionId);
                Assert.Equal(0, output.ExitCode);
                Assert.False(string.IsNullOrEmpty(output.Stdout));
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestAcknowledgeMessage(bool customFlow)
        {
            string topicId = "testTopicForMessageAck";
            string subscriptionId = "testSubscriptionForMessageAck";
            string message = TestUtil.RandomName();
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            var messageCreateOutput = Run("publishMessages", _projectId,
                topicId, message);
            // Pull and acknowldge the messages
            List<string> cmdArgs = new[] {"pullMessages", "-a", _projectId,
                    subscriptionId }.ToList();
            if (customFlow)
            {
                cmdArgs.Insert(2, "-f");
            }
            string[] cmd = cmdArgs.ToArray();
            Eventually(() =>
            {
                var output = Run(cmd);
                Assert.Equal(0, output.ExitCode);
                Assert.Contains(message, output.Stdout);
            });
            Eventually(() =>
            {
                //Pull the Message to confirm it's gone after it's acknowledged
                var output = Run(cmd);
                Assert.Equal(0, output.ExitCode);
                Assert.True(string.IsNullOrEmpty(output.Stdout));
            });
        }

        [Fact]
        public void TestListTopics()
        {
            string topicId = "testTopicForListingTopics";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            Eventually(() =>
            {
                var listProjectTopicsOutput = Run("listProjectTopics", _projectId);
                Assert.Equal(0, listProjectTopicsOutput.ExitCode);
                Assert.Contains(topicId, listProjectTopicsOutput.Stdout);
            });
        }

        [Fact]
        public void TestListTopicsWithServiceCredentials()
        {
            string topicId = "testTopicForListingTopics";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            Eventually(() =>
            {
                var listProjectTopicsOutput = Run("listProjectTopics", "-j",
                    Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"),
                    _projectId);
                Assert.Equal(0, listProjectTopicsOutput.ExitCode);
                Assert.Contains(topicId, listProjectTopicsOutput.Stdout);
            });
        }

        [Fact]
        public void TestListSubscriptions()
        {
            string topicId = "testTopicForListingSubscriptions";
            string subscriptionId = "testSubscriptionForListingSubscriptions";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription",
                _projectId,
                topicId, subscriptionId);
            Eventually(() =>
            {
                var listProjectSubscriptionsOutput = Run("listSubscriptions",
                    _projectId);
                Assert.Equal(0, listProjectSubscriptionsOutput.ExitCode);
                Assert.Contains(subscriptionId,
                    listProjectSubscriptionsOutput.Stdout);
            });
        }

        [Fact]
        public void TestGetTopicIamPolicy()
        {
            string topicId = "testTopicForGetTopicIamPolicy";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var policyOutput = Run("getTopicIamPolicy", _projectId, topicId);
            Assert.NotEmpty(policyOutput.Stdout);
        }

        [Fact]
        public void TestGetSubscriptionPolicy()
        {
            string topicId = "testTopicGetSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionGetSubscriptionIamPolicy";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            var policyOutput = Run("getSubscriptionIamPolicy", _projectId,
                subscriptionId);
            Assert.NotEmpty(policyOutput.ToString());
        }

        [Fact]
        public void TestSetTopicIamPolicy()
        {
            string topicId = "testTopicForSetTopicIamPolicy";
            string testRoleValueToConfirm = "pubsub.editor";
            string testMemberValueToConfirm = "group:cloud-logs@google.com";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var setTopicIamPolicyOutput = Run("setTopicIamPolicy", _projectId,
                topicId, testRoleValueToConfirm, testMemberValueToConfirm);
            var policyOutput = Run("getTopicIamPolicy", _projectId, topicId);
            Assert.Contains(testRoleValueToConfirm, policyOutput.Stdout);
            Assert.Contains(testMemberValueToConfirm, policyOutput.Stdout);
        }

        [Fact]
        public void TestSetSubscriptionIamPolicy()
        {
            string topicId = "testTopicSetSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionSetSubscriptionIamPolicy";
            string testRoleValueToConfirm = "pubsub.editor";
            string testMemberValueToConfirm = "group:cloud-logs@google.com";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            Run("setSubscriptionIamPolicy", _projectId,
                subscriptionId, testRoleValueToConfirm, testMemberValueToConfirm);
            var policyOutput = Run("getSubscriptionIamPolicy", _projectId,
                subscriptionId);
            Assert.Contains(testRoleValueToConfirm, policyOutput.Stdout);
            Assert.Contains(testMemberValueToConfirm, policyOutput.Stdout);
        }

        [Fact]
        public void TestTopicIamPolicyPermissions()
        {
            string topicId = "testTopicForTestTopicIamPolicy";
            string testRoleValueToConfirm = "pubsub.editor";
            string testMemberValueToConfirm = "group:cloud-logs@google.com";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var setTopicIamPolicyOutput = Run("setTopicIamPolicy", _projectId,
                topicId, testRoleValueToConfirm, testMemberValueToConfirm);
            TestIamPermissionsResponse response =
                TestTopicIamPermissionsResponse(topicId, _publisher);
            Assert.NotEmpty(response.ToString());
        }

        [Fact]
        public void TestSubscriptionPolicyPermisssions()
        {
            string topicId = "testTopicForTestSubscriptionIamPolicy";
            string subscriptionId = "testSubscriptionForTestSubscriptionIamPolicy";
            string testRoleValueToConfirm = "pubsub.editor";
            string testMemberValueToConfirm = "group:cloud-logs@google.com";
            var topicCreateOutput = Run("createTopic", _projectId, topicId);
            var subcriptionCreateOutput = Run("createSubscription", _projectId,
                topicId, subscriptionId);
            Run("setSubscriptionIamPolicy", _projectId,
                subscriptionId, testRoleValueToConfirm, testMemberValueToConfirm);
            TestIamPermissionsResponse response =
                TestSubscriptionIamPermissionsResponse(subscriptionId, _publisher);
            Assert.NotEmpty(response.ToString());
        }

        [Fact]
        public void TestRpcRetry()
        {
            string topicId = "testTopicForRpcRetry";
            string subscriptionId = "testSubscriptionForRpcRetry";
            _tempTopicIds.Add(topicId);
            _tempSubscriptionIds.Add(subscriptionId);
            RpcRetry(topicId, subscriptionId, _publisher, _subscriber);
            var topicDetails = Run("getTopic", _projectId, topicId);
            Assert.Contains($"{topicId}", topicDetails.Stdout);
        }

        [Fact]
        public void TestDeleteTopic()
        {
            string topicId = "testTopicForDeleteTopic";
            Run("createTopic", _projectId, topicId);
            Run("deleteTopic", _projectId, topicId);
            _tempTopicIds.Clear();  // We already deleted it.
            TopicName topicName = new TopicName(_projectId, topicId);
            Exception ex = Assert.Throws<Grpc.Core.RpcException>(() =>
                _publisher.GetTopic(topicName));
        }

        [Fact]
        public void TestDeleteSubscription()
        {
            string topicId = "testTopicForDeleteSubscription";
            string subscriptionId = "testSubscriptionForDeleteSubscription";
            Run("createTopic", _projectId, topicId);
            Run("createSubscription", _projectId, topicId, subscriptionId);
            Run("deleteSubscription", _projectId, subscriptionId);
            _tempSubscriptionIds.Clear();  // We already deleted it.
            SubscriptionName subscriptionName = new SubscriptionName(
                _projectId, subscriptionId);
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                _subscriber.GetSubscription(subscriptionName));
        }
    }
        
    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRun()
        {
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
        }
    }
}
