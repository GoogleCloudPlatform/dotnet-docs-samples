// Copyright 2017 Google Inc.
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

using CommandLine;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    [Verb("createTopic", HelpText = "Create a pubsub topic in this project.")]
    class CreateTopicOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to create.", Required = true)]
        public string topicId { get; set; }
    }

    [Verb("createSubscription", HelpText = "Create a pubsub subscription in this project.")]
    class CreateSubscriptionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to create a subscription for.", Required = true)]
        public string topicId { get; set; }
        [Value(2, HelpText = "The subscription to create.", Required = true)]
        public string subscriptionId { get; set; }
    }

    [Verb("publishMessages", HelpText = "Publish messages to a topic.")]
    class PublishMessageOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to publish a message to.", Required = true)]
        public string topicId { get; set; }
        [Value(2, HelpText = "The messages to publish to the topic.", Required = true)]
        public IEnumerable<string> message { get; set; }
        [Option('b', HelpText = "Use custom batch thresholds.", Default = false)]
        public bool customBatchThresholds { get; set; }
    }

    [Verb("pullMessages", HelpText = "Pull pubsub messages in this project.")]
    class PullMessagesOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to pull messages from.", Required = true)]
        public string subscriptionId { get; set; }
        [Option('a', HelpText = @"Acknowledge the pulled messages?", Default = false)]
        public bool acknowledge { get; set; }
        [Option('f', HelpText = @"Use custom flow control settings.", Default = false)]
        public bool customFlow { get; set; }
    }

    [Verb("getTopic", HelpText = "Get the details of a pubsub topic in this project.")]
    class GetTopicOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to get the details for.", Required = true)]
        public string topicId { get; set; }
    }

    [Verb("getSubscription", HelpText = "Get the details of a pubsub subscription in this project.")]
    class GetSubscriptionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to get the details for.", Required = true)]
        public string subscriptionId { get; set; }
    }

    [Verb("getTopicIamPolicy", HelpText = "Get the IAM policy of a topic in this project.")]
    class GetTopicIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to get the IAM Policy details for.", Required = true)]
        public string topicId { get; set; }
    }

    [Verb("getSubscriptionIamPolicy", HelpText = "Get the IAM policy of a subscription in this project.")]
    class GetSubscriptionIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to get the IAM Policy details for.", Required = true)]
        public string subscriptionId { get; set; }
    }

    [Verb("setTopicIamPolicy", HelpText = "Set the IAM policy of a topic in this project.")]
    class SetTopicIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to set the IAM Policy details for.", Required = true)]
        public string topicId { get; set; }
        [Value(2, HelpText = @"The role to set IAM policy for: 
        ""pubsub.viewer"" or ""pubsub.editor""", Required = true)]
        public string role { get; set; }
        [Value(3, HelpText = @"The member to add to the topic's IAM policy:
        Use one of the following formats for
        depending on the account type you are granting access to:
       ""user:alice@example.com""
       ""group:admins@example.com""
       ""domain:google.com""
       ""serviceAccount:my-other-app@appspot.gserviceaccount.com""
       ""allAuthenticatedUsers""
       ""allUsers""
       ", Required = true)]
        public string member { get; set; }
    }

    [Verb("setSubscriptionIamPolicy", HelpText = "Set the IAM policy of a subscription in this project.")]
    class SetSubscriptionIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to set the IAM Policy details for.", Required = true)]
        public string subscriptionId { get; set; }
        [Value(2, HelpText = @"The role to set IAM policy for: 
        ""pubsub.viewer"" or ""pubsub.editor""", Required = true)]
        public string role { get; set; }
        [Value(3, HelpText = @"The member to add to the subscription's IAM policy:
        Use one of the following formats for
        depending on the account type you are granting access to:
       ""user:alice@example.com""
       ""group:admins@example.com""
       ""domain:google.com""
       ""serviceAccount:my-other-app@appspot.gserviceaccount.com""
       ""allAuthenticatedUsers""
       ""allUsers""
       ", Required = true)]
        public string member { get; set; }
    }

    [Verb("listProjectTopics", HelpText = "List the pubsub topics in this project.")]
    class ListProjectTopicsOptions
    {
        [Value(0, HelpText = "The project ID of the project to list topics for.", Required = true)]
        public string projectId { get; set; }
        [Option('j', HelpText = "Path to a service credententials json file.", Required = false, Default = null)]
        public string serviceCredentialsJson { get; set; }
    }

    [Verb("listSubscriptions", HelpText = "List the pubsub subscriptions in this project.")]
    class ListSubscriptionsOptions
    {
        [Value(0, HelpText = "The project ID of the project to list subscriptions for.", Required = true)]
        public string projectId { get; set; }
    }

    [Verb("deleteSubscription", HelpText = "Delete a pubsub subscription in this project.")]
    class DeleteSubscriptionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to delete.", Required = true)]
        public string subscriptionId { get; set; }
    }

    [Verb("deleteTopic", HelpText = "Delete a pubsub topic in this project.")]
    class DeleteTopicOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to delete.", Required = true)]
        public string topicId { get; set; }
    }

    public class Pubsub
    {
        public static object CreateTopic(string projectId, string topicId)
        {
            // [START create_publisher_client]
            PublisherClient publisher = PublisherClient.Create();
            // [END create_publisher_client]

            // [START create_topic]
            TopicName topicName = new TopicName(projectId, topicId);
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
            return 0;
        }

        public static object CreateSubscription(string projectId, string topicId,
            string subscriptionId)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            // [START create_subscription]
            TopicName topicName = new TopicName(projectId, topicId);
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
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
            return 0;
        }

        public static SimplePublisher GetSimplePublisher(string projectId,
            string topicId)
        {
            // [START publish_message]
            PublisherClient publisherClient = PublisherClient.Create();
            SimplePublisher publisher = SimplePublisher.Create(
                new TopicName(projectId, topicId), new[] { publisherClient });
            // [END publish_message]
            return publisher;
        }

        /// <summary>
        /// Create a SimplePublisher with custom batch thresholds.
        /// </summary>
        public static SimplePublisher GetCustomPublisher(string projectId,
            string topicId)
        {
            // [START pubsub_publisher_batch_settings]
            PublisherClient publisherClient = PublisherClient.Create();
            SimplePublisher publisher = SimplePublisher.Create(
                new TopicName(projectId, topicId), new[] { publisherClient },
                new SimplePublisher.Settings()
                {
                    BatchingSettings = new Google.Api.Gax.BatchingSettings(
                        elementCountThreshold: 100,
                        byteCountThreshold: 10240,
                        delayThreshold: TimeSpan.FromSeconds(3))
                });
            // [END pubsub_publisher_batch_settings]
            return publisher;
        }

        public static object PublishMessages(SimplePublisher publisher,
            IEnumerable<string> messageTexts)
        {
            // [START publish_message]
            // [START pubsub_publisher_batch_settings]
            var publishTasks = new List<Task<string>>();
            // SimplePublisher collects messages into appropriately sized
            // batches.
            foreach (string text in messageTexts)
            {
                publishTasks.Add(publisher.PublishAsync(text));
            }
            foreach (var task in publishTasks)
            {
                Console.WriteLine("Published message {0}", task.Result);
            }
            // [END pubsub_publisher_batch_settings]
            // [END publish_message]
            return 0;
        }

        static SimpleSubscriber GetSimpleSubscriber(string projectId,
            string subscriptionId)
        {
            // [START pull_messages]
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
                subscriptionId);
            SubscriberClient subscriberClient = SubscriberClient.Create();
            SimpleSubscriber subscriber = SimpleSubscriber.Create(
                subscriptionName, new[] { subscriberClient });
            // [END pull_messages]
            return subscriber;
        }

        /// <summary>
        /// Create a subscriber with custom control flow settings.
        /// </summary>
        static SimpleSubscriber GetCustomSubscriber(string projectId,
            string subscriptionId)
        {
            // [START pubsub_subscriber_flow_settings]
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
                subscriptionId);
            SubscriberClient subscriberClient = SubscriberClient.Create();
            SimpleSubscriber subscriber = SimpleSubscriber.Create(
                subscriptionName, new[] { subscriberClient },
                new SimpleSubscriber.Settings()
                {
                    AckExtensionWindow = TimeSpan.FromSeconds(4),
                    Scheduler = Google.Api.Gax.SystemScheduler.Instance,
                    StreamAckDeadline = TimeSpan.FromSeconds(10),
                    FlowControlSettings = new Google.Api.Gax
                        .FlowControlSettings(
                        maxOutstandingElementCount: 100,
                        maxOutstandardByteCount: 10240)
                });
            // [END pubsub_subscriber_flow_settings]
            return subscriber;
        }

        public static object PullMessages(SimpleSubscriber subscriber, bool acknowledge)
        {
            // [START pull_messages]
            // [START pubsub_subscriber_flow_settings]
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            subscriber.StartAsync(
                async (PubsubMessage message, CancellationToken cancel) =>
                {
                    string text =
                        Encoding.UTF8.GetString(message.Data.ToArray());
                    await Console.Out.WriteLineAsync(
                        $"Message {message.MessageId}: {text}");
                    return acknowledge ? SimpleSubscriber.Reply.Ack
                        : SimpleSubscriber.Reply.Nack;
                });
            // Run for 3 seconds.
            Thread.Sleep(3000);
            subscriber.StopAsync(CancellationToken.None).Wait();
            // [END pubsub_subscriber_flow_settings]
            // [END pull_messages]
            return 0;
        }

        public static object GetTopic(string projectId, string topicId)
        {
            PublisherClient publisher = PublisherClient.Create();
            TopicName topicName = new TopicName(projectId, topicId);
            Topic topic = publisher.GetTopic(topicName);
            Console.WriteLine($"Topic found: {topic.TopicName.ToString()}");
            return 0;
        }

        public static object GetSubscription(string projectId,
            string subscriptionId)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
                subscriptionId);
            Subscription subscription = subscriber.GetSubscription(
                subscriptionName);
            Console.WriteLine($"Subscription found:" +
                $"{subscription.SubscriptionName.ToString()}");
            return 0;
        }

        public static object GetTopicIamPolicy(string projectId, string topicId)
        {
            PublisherClient publisher = PublisherClient.Create();
            // [START pubsub_get_topic_policy]
            TopicName topicName = new TopicName(projectId, topicId);
            Policy policy = publisher.GetIamPolicy(topicName.ToString());
            Console.WriteLine($"Topic IAM Policy found for {topicId}:");
            Console.WriteLine(policy.Bindings);
            // [END pubsub_get_topic_policy]
            return 0;
        }

        public static object GetSubscriptionIamPolicy(string projectId,
            string subscriptionId)
        {
            PublisherClient publisher = PublisherClient.Create();
            // [START pubsub_get_subscription_policy]
            SubscriptionName subscriptionName = new SubscriptionName(projectId, subscriptionId);
            Policy policy = publisher.GetIamPolicy(subscriptionName.ToString());
            Console.WriteLine($"Subscription IAM Policy found for {subscriptionId}:");
            Console.WriteLine(policy.Bindings);
            // [END pubsub_get_subscription_policy]
            return 0;
        }

        public static object SetTopicIamPolicy(string projectId,
            string topicId, string role, string member)
        {
            PublisherClient publisher = PublisherClient.Create();
            string roleToBeAddedToPolicy = $"roles/{role}";
            // [START pubsub_set_topic_policy]
            Policy policy = new Policy
            {
                Bindings =
                    {
                        new Binding { Role = roleToBeAddedToPolicy,
                            Members = { member } }
                    }
            };
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = new TopicName(projectId, topicId).ToString(),
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            Console.WriteLine($"Topic IAM Policy updated: {response}");
            // [END pubsub_set_topic_policy]
            return 0;
        }

        public static object SetSubscriptionIamPolicy(string projectId,
            string subscriptionId, string role, string member)
        {
            PublisherClient publisher = PublisherClient.Create();
            string roleToBeAddedToPolicy = $"roles/{role}";
            // [START pubsub_set_subscription_policy]
            Policy policy = new Policy
            {
                Bindings =
                {
                    new Binding { Role = roleToBeAddedToPolicy,
                        Members = { member } }
                }
            };
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = new SubscriptionName(projectId, subscriptionId).ToString(),
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            Console.WriteLine($"Subscription IAM Policy updated: {response}");
            // [END pubsub_set_subscription_policy]
            return 0;
        }

        public static object ListProjectTopics(PublisherClient publisher, string projectId)
        {
            // [START list_topics]
            ProjectName projectName = new ProjectName(projectId);
            IEnumerable<Topic> topics = publisher.ListTopics(projectName);
            // [END list_topics]         
            foreach (Topic topic in topics)
            {
                Console.WriteLine($"{topic.Name}");
            }
            return 0;
        }

        /// <summary>
        /// Creates a PublisherClient given a path to a downloaded json service
        /// credentials file.
        /// </summary>
        /// <param name="jsonPath">The path to the downloaded json file.</param>
        /// <returns>A new publisher client.</returns>
        public static PublisherClient CreatePublisherWithServiceCredentials(
            string jsonPath)
        {
            GoogleCredential googleCredential = null;
            using (var jsonStream = new FileStream(jsonPath, FileMode.Open,
                FileAccess.Read, FileShare.Read))
            {
                googleCredential = GoogleCredential.FromStream(jsonStream)
                    .CreateScoped(PublisherClient.DefaultScopes);
            }
            Channel channel = new Channel(PublisherClient.DefaultEndpoint.Host,
                PublisherClient.DefaultEndpoint.Port,
                googleCredential.ToChannelCredentials());
            return PublisherClient.Create(channel);
        }

        public static object ListSubscriptions(string projectId)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            // [START list_subscriptions]
            ProjectName projectName = new ProjectName(projectId);
            IEnumerable<Subscription> subscriptions =
                subscriber.ListSubscriptions(projectName);
            // [END list_subscriptions]
            foreach (Subscription subscription in subscriptions)
            {
                Console.WriteLine($"{subscription}");
            }
            return 0;
        }

        public static object DeleteSubscription(string projectId,
            string subscriptionId)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            // [START delete_subscription]
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
                subscriptionId);
            subscriber.DeleteSubscription(subscriptionName);
            // [END delete_subscription]
            Console.WriteLine("Subscription deleted.");
            return 0;
        }

        public static object DeleteTopic(string projectId, string topicId)
        {
            PublisherClient publisher = PublisherClient.Create();
            // [START delete_topic]
            TopicName topicName = new TopicName(projectId, topicId);
            publisher.DeleteTopic(topicName);
            // [END delete_topic]
            Console.WriteLine("Topic deleted.");
            return 0;
        }


        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CreateTopicOptions, CreateSubscriptionOptions,
                PublishMessageOptions, PullMessagesOptions,
                GetTopicOptions, GetSubscriptionOptions,
                GetTopicIamPolicyOptions, GetSubscriptionIamPolicyOptions,
                SetTopicIamPolicyOptions, SetSubscriptionIamPolicyOptions,
                ListProjectTopicsOptions, ListSubscriptionsOptions,
                DeleteSubscriptionOptions, DeleteTopicOptions
                >(args)
                .MapResult(
                (CreateTopicOptions opts) => CreateTopic(
                  opts.projectId, opts.topicId),
                (CreateSubscriptionOptions opts) => CreateSubscription(opts.projectId,
                opts.topicId, opts.subscriptionId),
                (PublishMessageOptions opts) => PublishMessages(opts.customBatchThresholds
                    ? GetCustomPublisher(opts.projectId, opts.topicId)
                    : GetSimplePublisher(opts.projectId, opts.topicId), opts.message),
                (PullMessagesOptions opts) => PullMessages(opts.customFlow
                    ? GetCustomSubscriber(opts.projectId, opts.subscriptionId)
                    : GetSimpleSubscriber(opts.projectId, opts.subscriptionId), opts.acknowledge),
                (GetTopicOptions opts) => GetTopic(opts.projectId, opts.topicId),
                (GetSubscriptionOptions opts) => GetSubscription(opts.projectId,
                opts.subscriptionId),
                (GetTopicIamPolicyOptions opts) => GetTopicIamPolicy(opts.projectId, opts.topicId),
                (GetSubscriptionIamPolicyOptions opts) => GetSubscriptionIamPolicy(opts.projectId,
                opts.subscriptionId),
                (SetTopicIamPolicyOptions opts) => SetTopicIamPolicy(opts.projectId,
                opts.topicId, opts.role, opts.member),
                (SetSubscriptionIamPolicyOptions opts) => SetSubscriptionIamPolicy(opts.projectId,
                opts.subscriptionId, opts.role, opts.member),
                (ListProjectTopicsOptions opts) => ListProjectTopics(
                    string.IsNullOrWhiteSpace(opts.serviceCredentialsJson)
                    ? PublisherClient.Create()
                    : CreatePublisherWithServiceCredentials(opts.serviceCredentialsJson),
                    opts.projectId),
                (ListSubscriptionsOptions opts) => ListSubscriptions(opts.projectId),
                (DeleteSubscriptionOptions opts) => DeleteSubscription(opts.projectId, opts.subscriptionId),
                (DeleteTopicOptions opts) => DeleteTopic(
                  opts.projectId, opts.topicId),
                errs => 1);
        }
    }
}
