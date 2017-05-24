using CommandLine;
using Google.Api.Gax.Grpc;
using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

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

    [Verb("createTopicMessage", HelpText = "Create a pubsub topic message in this project.")]
    class CreateTopicMessageOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The topic to publish a message to.", Required = true)]
        public string topicId { get; set; }
        [Value(2, HelpText = "The message to publish to the topic.", Required = true)]
        public string message { get; set; }
        [Value(3, HelpText = "The optional attributes key to associate with the published message", Required = false)]
        public string attributesKey { get; set; }
        [Value(4, HelpText = "The optional attributes value to associate with the published message", Required = false)]
        public string attributesValue { get; set; }
    }

    [Verb("pullTopicMessages", HelpText = "Pull pubsub messages in this project.")]
    class PullTopicMessagesOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for pubsub operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The subscription to pull messages from.", Required = true)]
        public string subscriptionId { get; set; }
        [Value(2, HelpText = @"Acknowledge the pulled messages? Use ""true"" or ""false"".", Default = false)]
        public bool acknowledge { get; set; }
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



    public class Program
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

        public static object CreateTopicMessage(string projectId,
            string topicId, string messageText,
            string attributesKey = "description", string attributesValue = "")
        {
            PublisherClient publisher = PublisherClient.Create();
            // [START publish_message]
            TopicName topicName = new TopicName(projectId, topicId);
            PubsubMessage message = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(messageText),
                // The attributes provide metadata in a string-to-string 
                // dictionary.
                Attributes =
                {
                    { attributesKey, attributesValue }
                }
            };
            publisher.Publish(topicName, new[] { message });
            Console.WriteLine("Topic message created.");
            // [END publish_message]
            return 0;
        }

        public static object PullTopicMessages(string projectId,
            string subscriptionId, bool acknowledge)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            // [START pull_messages]
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
                subscriptionId);
            PullResponse response = subscriber.Pull(subscriptionName,
                returnImmediately: true, maxMessages: 10);
            // [END pull_messages]
            if (response.ReceivedMessages.Count > 0)
            {
                foreach (ReceivedMessage message in response.ReceivedMessages)
                {
                    Console.WriteLine($"Message {message.AckId}: " +
                        $"{message.Message}");
                }
                if (acknowledge)
                {
                    AcknowledgeTopicMessage(projectId, subscriptionId, response);
                    Console.WriteLine($"# of Messages received and acknowledged:" +
                        $" {response.ReceivedMessages.Count}");
                }
                else
                {
                    Console.WriteLine($"# of Messages received:" +
                        $" {response.ReceivedMessages.Count}");
                }
            }
            return 0;
        }

        public static object AcknowledgeTopicMessage(string projectId,
            string subscriptionId, PullResponse response)
        {
            SubscriberClient subscriber = SubscriberClient.Create();
            SubscriptionName subscriptionName = new SubscriptionName(projectId,
     subscriptionId);
            // [START pull_messages]
            subscriber.Acknowledge(subscriptionName,
                response.ReceivedMessages.Select(m => m.AckId));
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
            Console.WriteLine($"{ policy.Bindings.ToString()}");
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
            Console.WriteLine($"{ policy.Bindings.ToString()}");
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

        public static object ListProjectTopics(string projectId)
        {
            PublisherClient publisher = PublisherClient.Create();
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
                CreateTopicMessageOptions, PullTopicMessagesOptions,
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
                (CreateTopicMessageOptions opts) => CreateTopicMessage(opts.projectId,
                opts.topicId, opts.message),
                (PullTopicMessagesOptions opts) => PullTopicMessages(opts.projectId,
                opts.subscriptionId, opts.acknowledge),
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
                (ListProjectTopicsOptions opts) => ListProjectTopics(opts.projectId),
                (ListSubscriptionsOptions opts) => ListSubscriptions(opts.projectId),
                (DeleteSubscriptionOptions opts) => DeleteSubscription(opts.projectId, opts.subscriptionId),
                (DeleteTopicOptions opts) => DeleteTopic(
                  opts.projectId, opts.topicId),
                errs => 1);
        }
    }
}
