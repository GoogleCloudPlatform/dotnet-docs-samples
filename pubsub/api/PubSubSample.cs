/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Google.Apis.Services;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleCloudSamples
{
    public class PubSubSample
    {
        public PubsubService CreatePubSubClient()
        {
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
            credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = "PubSub Sample",
                HttpClientInitializer = credentials
            };

            return new PubsubService(serviceInitializer);
        }

        public void CreateTopic(string projectId, string topicName)
        {
            PubsubService PubSub = CreatePubSubClient();

            Topic topic = PubSub.Projects.Topics.Create(
              name: $"projects/{projectId}/topics/{topicName}",
              body: new Topic() { Name = topicName }
            ).Execute();

            Console.WriteLine($"Created: {topic.Name}");
        }

        // TODO Add PushConfig
        public void CreateSubscription(string projectId, string topicName, string subscriptionName)
        {
            PubsubService PubSub = CreatePubSubClient();

            Subscription subscription = PubSub.Projects.Subscriptions.Create(
              name: $"projects/{projectId}/subscriptions/{subscriptionName}",
              body: new Subscription()
              {
                  Name = subscriptionName,
                  Topic = $"projects/{projectId}/topics/{topicName}"
              }
            ).Execute();

            Console.WriteLine($"Created: {subscription.Name}");
        }

        public void ListTopics(string projectId)
        {
            PubsubService PubSub = CreatePubSubClient();

            ListTopicsResponse response = PubSub.Projects.Topics.List(
              project: $"projects/{projectId}"
            ).Execute();

            if (response.Topics != null)
            {
                IList<Topic> topics = response.Topics;

                foreach (var topic in topics)
                {
                    Console.WriteLine($"Found topics: {topic.Name}");
                }
            }
        }

        public void ListSubscriptions(string projectId)
        {
            PubsubService PubSub = CreatePubSubClient();

            ListSubscriptionsResponse response = PubSub.Projects.Subscriptions.List(
              project: $"projects/{projectId}"
            ).Execute();

            if (response.Subscriptions != null)
            {
                IList<Subscription> subscriptions = response.Subscriptions;

                foreach (var subscription in subscriptions)
                {
                    Console.WriteLine($"Found subscription: {subscription.Name}");
                }
            }
        }

        public void PublishMessage(string projectId, string topicName, string message)
        {
            PubsubService PubSub = CreatePubSubClient();

            message = Convert.ToBase64String(
              Encoding.UTF8.GetBytes(message)
            );

            PublishResponse response = PubSub.Projects.Topics.Publish(
              topic: $"projects/{projectId}/topics/{topicName}",
              body: new PublishRequest()
              {
                  Messages = new[]
                {
          new PubsubMessage() { Data = message }
                }
              }
            ).Execute();

            if (response.MessageIds != null)
            {
                IList<string> messageIds = response.MessageIds;

                foreach (var messageId in messageIds)
                {
                    Console.WriteLine($"Published message ID: {messageId}");
                }
            }
        }

        public void Pull(string projectId, string subscriptionName)
        {
            PubsubService PubSub = CreatePubSubClient();

            PullResponse response = PubSub.Projects.Subscriptions.Pull(
              subscription: $"projects/{projectId}/subscriptions/{subscriptionName}",
              body: new PullRequest()
              {
                  MaxMessages = 10,
                  ReturnImmediately = true
              }
            ).Execute();

            if (response.ReceivedMessages != null)
            {
                IList<ReceivedMessage> receivedMessages = response.ReceivedMessages;

                // Print out all messages
                foreach (var receivedMessage in receivedMessages)
                {
                    string message = Encoding.UTF8.GetString(
                      Convert.FromBase64String(receivedMessage.Message.Data)
                    );

                    Console.WriteLine(message);
                }

                // Acknowledge receipt of all messages
                PubSub.Projects.Subscriptions.Acknowledge(
                  subscription: $"projects/{projectId}/subscriptions/{subscriptionName}",
                  body: new AcknowledgeRequest()
                  {
                      AckIds = receivedMessages.Select(m => m.AckId).ToList()
                  }
                ).Execute();
            }
            else
            {
                Console.WriteLine("There were no messages");
            }
        }

        public void GetTopicPolicy(string projectId, string topicName)
        {
            PubsubService PubSub = CreatePubSubClient();

            Policy policy = PubSub.Projects.Topics.GetIamPolicy(
              resource: $"projects/{projectId}/topics/{topicName}"
            ).Execute();

            if (policy.Bindings != null)
            {
                Console.WriteLine($"Policy {policy.Version} for subscription");

                foreach (Binding binding in policy.Bindings)
                {
                    foreach (string member in binding.Members)
                    {
                        Console.WriteLine($"{member} is member of role {binding.Role}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Topic has no policy");
            }
        }

        public void GetSubscriptionPolicy(string projectId, string subscriptionName)
        {
            PubsubService PubSub = CreatePubSubClient();

            Policy policy = PubSub.Projects.Subscriptions.GetIamPolicy(
              resource: $"projects/{projectId}/subscriptions/{subscriptionName}"
            ).Execute();

            if (policy.Bindings != null)
            {
                foreach (Binding binding in policy.Bindings)
                {
                    foreach (string member in binding.Members)
                    {
                        Console.WriteLine($"{member} is member of role {binding.Role}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Subscription has no policy");
            }
        }

        public void SetTopicPolicy(string projectId, string topicName, IDictionary<string, string[]> rolesAndMembers)
        {
            PubsubService PubSub = CreatePubSubClient();

            IList<Binding> bindings = new List<Binding>();

            foreach (var roleName in rolesAndMembers.Keys)
                bindings.Add(new Binding() { Role = roleName, Members = rolesAndMembers[roleName] });

            Policy policy = PubSub.Projects.Topics.SetIamPolicy(
              resource: $"projects/{projectId}/topics/{topicName}",
              body: new SetIamPolicyRequest() { Policy = new Policy() { Bindings = bindings } }
            ).Execute();

            Console.WriteLine("Set policy");
            foreach (var binding in policy.Bindings)
            {
                Console.WriteLine($"Role: {binding.Role}");
                foreach (var theMember in binding.Members)
                    Console.WriteLine($" - {theMember}");
            }
        }

        // TODO demonstrate clearly how to create simple policy bindings.
        // USAGE serviceAccount:myproject@appspot.gserviceaccount.com roles/pubsub.subscriber
        public void SetSubscriptionPolicy(string projectId, string subscriptionName, IDictionary<string, string[]> rolesAndMembers)
        {
            PubsubService PubSub = CreatePubSubClient();

            IList<Binding> bindings = new List<Binding>();

            foreach (var roleName in rolesAndMembers.Keys)
                bindings.Add(new Binding() { Role = roleName, Members = rolesAndMembers[roleName] });

            Policy policy = PubSub.Projects.Subscriptions.SetIamPolicy(
              resource: $"projects/{projectId}/subscriptions/{subscriptionName}",
              body: new SetIamPolicyRequest() { Policy = new Policy() { Bindings = bindings } }
            ).Execute();
        }

        public void TestTopicPermissions(string projectId, string topicName, string[] permissions)
        {
            PubsubService PubSub = CreatePubSubClient();

            TestIamPermissionsResponse response = PubSub.Projects.Topics.TestIamPermissions(
              resource: $"projects/{projectId}/topics/{topicName}",
              body: new TestIamPermissionsRequest() { Permissions = permissions }
            ).Execute();

            foreach (var permission in permissions)
            {
                if (response.Permissions.Contains(permission))
                    Console.WriteLine($"Caller has permission {permission}");
                else
                    Console.WriteLine($"Caller does not have persmission {permission}");
            }
        }

        public void TestSubscriptionPermissions(string projectId, string subscriptionName, string[] permissions)
        {
            PubsubService PubSub = CreatePubSubClient();

            TestIamPermissionsResponse response = PubSub.Projects.Subscriptions.TestIamPermissions(
              resource: $"projects/{projectId}/subscriptions/{subscriptionName}",
              body: new TestIamPermissionsRequest() { Permissions = permissions }
            ).Execute();

            foreach (var permission in permissions)
            {
                if (response.Permissions.Contains(permission))
                    Console.WriteLine($"Caller has permission {permission}");
                else
                    Console.WriteLine($"Caller does not have persmission {permission}");
            }
        }
    }
}
