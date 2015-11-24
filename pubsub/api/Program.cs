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

using System;
using System.Linq;
using System.Collections.Generic;

namespace GoogleCloudSamples
{
    public class PubSubProgram
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                PrintUsage();
            else
                RunCommand(args.First(), args.Skip(1).ToArray());
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"Usage: PubSubSample.exe [command] [args]

       ListTopics
       ListSubscriptions
       CreateTopic                 [name]
       CreateSubscription          [topic] [name]
       PublishMessage              [topic] [message]
       Pull                        [subscription]
       GetTopicPolicy              [topic]
       GetSubscriptionPolicy       [subscription]
       SetTopicPolicy              [topic]        [role=member,member...] [role=...]
       SetSubscriptionPolicy       [subscription] [role=member,member...] [role=...]
       TestTopicPermissions        [topic]        [permission] [permission...]
       TestSubscriptionPermissions [subscription] [permission] [permission...]
");
        }

        private static void RunCommand(string command, string[] args)
        {
            var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            switch (command)
            {
                case "ListTopics":
                    new PubSubSample().ListTopics(projectId);
                    break;

                case "ListSubscriptions":
                    new PubSubSample().ListSubscriptions(projectId);
                    break;

                case "CreateTopic":
                    new PubSubSample().CreateTopic(projectId,
                      topicName: args[0]
                    );
                    break;

                case "CreateSubscription":
                    new PubSubSample().CreateSubscription(projectId,
                      topicName: args[0],
                      subscriptionName: args[1]
                    );
                    break;

                case "PublishMessage":
                    new PubSubSample().PublishMessage(projectId,
                      topicName: args[0],
                      message: args[1]
                    );
                    break;

                case "Pull":
                    new PubSubSample().Pull(projectId, subscriptionName: args[0]);
                    break;

                case "GetTopicPolicy":
                    new PubSubSample().GetTopicPolicy(projectId, topicName: args[0]);
                    break;

                case "GetSubscriptionPolicy":
                    new PubSubSample().GetSubscriptionPolicy(projectId, subscriptionName: args[0]);
                    break;

                case "SetTopicPolicy":
                    var topicName = args[0];
                    var topicPolicyArguments = args.Skip(1);

                    var topicRolesAndMembers = new Dictionary<string, string[]>();

                    foreach (var arg in topicPolicyArguments)
                    {
                        var roleName = arg.Split('=')[0];
                        var memberNames = arg.Split('=')[1].Split(',');
                        topicRolesAndMembers[roleName] = memberNames;
                    }

                    new PubSubSample().SetTopicPolicy(projectId,
                      topicName: topicName,
                      rolesAndMembers: topicRolesAndMembers
                    );
                    break;

                case "SetSubscriptionPolicy":
                    var subscriptionName = args[0];
                    var subscriptionPolicyArguments = args.Skip(1);

                    var subscriptionRolesAndMembers = new Dictionary<string, string[]>();

                    foreach (var arg in subscriptionPolicyArguments)
                    {
                        var roleName = arg.Split('=')[0];
                        var memberNames = arg.Split('=')[1].Split(',');
                        subscriptionRolesAndMembers[roleName] = memberNames;
                    }

                    new PubSubSample().SetSubscriptionPolicy(projectId,
                      subscriptionName: subscriptionName,
                      rolesAndMembers: subscriptionRolesAndMembers
                    );
                    break;

                case "TestTopicPermissions":
                    new PubSubSample().TestTopicPermissions(projectId,
                      topicName: args[0],
                      permissions: args.Skip(1).ToArray()
                    );
                    break;

                case "TestSubscriptionPermissions":
                    new PubSubSample().TestSubscriptionPermissions(projectId,
                      subscriptionName: args[0],
                      permissions: args.Skip(1).ToArray()
                    );
                    break;

                default:
                    Console.WriteLine($"Command not found: {command}");
                    break;
            }
        }
    }
}
