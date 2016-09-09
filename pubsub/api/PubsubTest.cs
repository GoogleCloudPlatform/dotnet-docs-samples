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
using Google.Pubsub.V1;
// [END create_publisher_client]
using System.Linq;
using Google.Protobuf;
using System.Collections.Generic;
using Xunit;

namespace GoogleCloudSamples
{
    public class PubsubTest
    {
        private readonly string _projectId;
        private readonly PublisherClient _publisher;
        private readonly SubscriberClient _subscriber;
        // [START retry]
        private readonly int _retryCount = 3;
        private readonly int _retryDelayMs = 500;
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
            string topicName = PublisherClient.FormatTopicName(_projectId, 
                topicId);
            publisher.CreateTopic(topicName);
            // [END create_topic]
        }

        public void CreateSubscription(string topicId, string subscriptionId, 
            SubscriberClient subscriber)
        {
            // [START create_subscription]
            string topicName = PublisherClient.FormatTopicName(_projectId, 
                topicId);
            string subscriptionName = 
                SubscriberClient.FormatSubscriptionName(_projectId, 
                subscriptionId);
            Subscription subscription = subscriber.CreateSubscription(
                subscriptionName, topicName, pushConfig: null, 
                ackDeadlineSeconds: 60);
            // [END create_subscription]
        }

        public void CreateTopicMessage(string topicId, PublisherClient publisher)
        {
            // [START publish_message]
            string topicName = PublisherClient.FormatTopicName(_projectId, 
                topicId);
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
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, 
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
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, 
                subscriptionId);
            // [START pull_messages]
            subscriber.Acknowledge(subscriptionName, 
                response.ReceivedMessages.Select(m => m.AckId));
            // [END pull_messages]
        }

        public Topic GetTopic(string topicId, PublisherClient publisher)
        {
            string topicName = PublisherClient.FormatTopicName(_projectId, 
                topicId);
            Topic topic = _publisher.GetTopic(topicName);
            return topic;
        }

        public Subscription GetSubscription(string subscriptionId, 
            SubscriberClient subscriber)
        {
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, 
                subscriptionId);
            Subscription subscription = _subscriber.GetSubscription(
                subscriptionName);
            return subscription;
        }

        public IEnumerable<Topic> ListProjectTopics(PublisherClient publisher)
        {
            // [START list_topics]
            string projectName = PublisherClient.FormatProjectName(_projectId);
            IEnumerable<Topic> topics = publisher.ListTopics(projectName);
            // [END list_topics]
            return topics;
        }

        public IEnumerable<Subscription> ListSubscriptions(SubscriberClient 
            subscriber)
        {
            // [START list_subscriptions]
            string projectName = PublisherClient.FormatProjectName(_projectId);
            IEnumerable<Subscription> subscriptions = 
                subscriber.ListSubscriptions(projectName);
            // [END list_subscriptions]
            return subscriptions;
        }

        public void DeleteSubscription(string subscriptionId, SubscriberClient 
            subscriber)
        {
            // [START delete_subscription]
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, 
                subscriptionId);
            subscriber.DeleteSubscription(subscriptionName);
            // [END delete_subscription]
        }

        public void DeleteTopic(string topicId, PublisherClient publisher)
        {
            // [START delete_topic]
            string topicName = PublisherClient.FormatTopicName(_projectId, 
                topicId);
            publisher.DeleteTopic(topicName);
            // [END delete_topic]
        }

        // [START retry]
        /// <summary>
        /// Retry the action when a Grpc.Core.RpcException is thrown.
        /// </summary>
        private T RetryRpc<T>(Func<T> action)
        {
            List<Grpc.Core.RpcException> exceptions = null;
            var delayMs = _retryDelayMs;
            for (int tryCount = 0; tryCount < _retryCount; ++tryCount)
            {
                try
                {
                    return action();
                }
                catch (Grpc.Core.RpcException e)
                {
                    if (exceptions == null)
                        exceptions = new List<Grpc.Core.RpcException>();
                    exceptions.Add(e);
                }
                System.Threading.Thread.Sleep(delayMs);
                delayMs *= 2;  // Exponential back-off.
            }
            throw new AggregateException(exceptions);
        }

        private void RetryRpc(Action action)
        {
            try {
                RetryRpc(() => { action(); return 0; });
            }
            catch (AggregateException exceptions)
            {
                throw exceptions;
            }
        }
        // [END retry]

        public void RpcRetry(string topicId, string subscriptionId,
            PublisherClient publisher, SubscriberClient subscriber)
        {
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
            try
            {
                RetryRpc(() =>
                {
                    // Subscribe to Topic
                    // This should fail since Topic has not yet been created
                    subscriber.CreateSubscription(subscriptionName, topicName,
                        pushConfig: null, ackDeadlineSeconds: 60);
                });
            }
            catch (AggregateException exceptions)
            {
                if (exceptions.InnerExceptions.Count() == _retryCount)
                {
                    // [START retry]
                    try
                    {
                        RetryRpc(() =>
                        {
                            //Create Topic
                            publisher.CreateTopic(topicName);
                        });
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.Flatten().InnerExceptions)
                        {                    
                            // A StatusCode of "AlreadyExists" is ok.  
                            // It means the topic already exists.
                            if(!e.Message.Contains("StatusCode=AlreadyExists"))
                            {
                                throw;
                            }                    
                        }
                    }
                    // [END retry]
                    try
                    {
                        RetryRpc(() =>
                        {
                            // Subscribe to Topic
                            subscriber.CreateSubscription(subscriptionName, topicName,
                                pushConfig: null, ackDeadlineSeconds: 60);
                        });
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.Flatten().InnerExceptions)
                        {
                            // A StatusCode of "AlreadyExists" is ok.  
                            // It means the subscription already exists.
                            if (!e.Message.Contains("StatusCode=AlreadyExists"))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        }

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
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
            Topic topic = GetTopic(topicId, _publisher);
            Assert.Equal(topicName, topic.Name);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestCreateSubscription()
        {
            string topicId = "testTopicForSubscriptionCreation";
            string subscriptionId = "testSubscriptionForSubscriptionCreation";
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            Subscription subscription = GetSubscription(subscriptionId, 
                _subscriber);
            Assert.Equal(subscriptionName, subscription.Name);
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId,_publisher);
        }

        [Fact]
        public void TestCreateTopicMessage()
        {
            string topicId = "testTopicForMessageCreation";
            string subscriptionId = "testSubscriptionForMessageCreation";
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
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
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
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
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
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
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            IEnumerable<Subscription> subscriptions = ListSubscriptions(
                _subscriber);
            Assert.False(subscriptions.Count() == 0);
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
            Subscription subscription = GetSubscription(subscriptionId,
                _subscriber);
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
            Assert.Equal(subscriptionName, subscription.Name);
            DeleteSubscription(subscriptionId, _subscriber);
            DeleteTopic(topicId, _publisher);
        }

        [Fact]
        public void TestDeleteTopic()
        {
            string topicId = "testTopicForDeleteTopic";
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
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
            string topicName = PublisherClient.FormatTopicName(_projectId, topicId);
            string subscriptionName =
                SubscriberClient.FormatSubscriptionName(_projectId, subscriptionId);
            CreateTopic(topicId, _publisher);
            CreateSubscription(topicId, subscriptionId, _subscriber);
            DeleteSubscription(subscriptionId, _subscriber);
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                _subscriber.GetSubscription(subscriptionName));
            DeleteTopic(topicId, _publisher);
        }
    }
}
