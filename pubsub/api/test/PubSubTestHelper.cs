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

using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class PubSubTestHelper
{
  public string ProjectID = System.Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
  public string ProjectResource { get { return $"projects/{ProjectID}"; } }
  public PubsubService Pubsub = PubSubClient.Create();

  public void DeleteAllSubscriptions()
  {
    var subscriptionsResponse = Pubsub.Projects.Subscriptions.List(ProjectResource).Execute();
    if (subscriptionsResponse.Subscriptions != null)
      foreach (var subscription in subscriptionsResponse.Subscriptions)
        Pubsub.Projects.Subscriptions.Delete(subscription.Name).Execute();
  }

  public void DeleteAllTopics()
  {
    var topicsResponse = Pubsub.Projects.Topics.List(ProjectResource).Execute();
    if (topicsResponse.Topics != null)
      foreach (var topic in topicsResponse.Topics)
        Pubsub.Projects.Topics.Delete(topic.Name).Execute();
  }

  public void CreateTopic(string name)
  {
    Pubsub.Projects.Topics.Create(new Topic() { Name = name }, $"{ProjectResource}/topics/{name}").Execute();
  }

  public void CreateSubscription(string topic, string name)
  {
    var subscription = new Subscription() { Name = name, Topic = $"{ProjectResource}/topics/{topic}" };
    Pubsub.Projects.Subscriptions.Create(subscription, $"{ProjectResource}/subscriptions/{name}").Execute();
  }

  public List<string> PullMessages(string subscription)
  {
    var messages = new List<string>();
    var response = Pubsub.Projects.Subscriptions.Pull(
      new PullRequest() { MaxMessages = 100, ReturnImmediately = true },
      $"{ProjectResource}/subscriptions/{subscription}" 
    ).Execute();

    foreach (var message in response.ReceivedMessages)
    {
      messages.Add(
        System.Text.Encoding.UTF8.GetString(
          System.Convert.FromBase64String(message.Message.Data)
        )
      );
    }

    return messages;
  }

  public void PublishMessage(string topic, string message)
  {
    var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));

    Pubsub.Projects.Topics.Publish(
      new PublishRequest() { Messages = new[] { new PubsubMessage() { Data = base64 } } },
      $"{ProjectResource}/topics/{topic}"
    ).Execute();
  }

  public void SetTopicPolicy(string topic, IDictionary<string, string[]> roleMembers)
  {
    var bindings = new List<Binding>();
    foreach (var role in roleMembers.Keys)
      bindings.Add(new Binding() { Role = role, Members = roleMembers[role] });

    Pubsub.Projects.Topics.SetIamPolicy(
      new SetIamPolicyRequest() { Policy = new Policy() { Bindings = bindings } },
      $"{ProjectResource}/topics/{topic}"
    ).Execute();
  }

  public void SetSubscriptionPolicy(string subscription, IDictionary<string, string[]> roleMembers)
  {
    var bindings = new List<Binding>();
    foreach (var role in roleMembers.Keys)
      bindings.Add(new Binding() { Role = role, Members = roleMembers[role] });

    Pubsub.Projects.Topics.SetIamPolicy(
      new SetIamPolicyRequest() { Policy = new Policy() { Bindings = bindings } },
      $"{ProjectResource}/subscriptions/{subscription}"
    ).Execute();
  }

  public Policy GetTopicPolicy(string topic)
  {
    return Pubsub.Projects.Topics.GetIamPolicy($"{ProjectResource}/topics/{topic}").Execute();
  }

  public Policy GetSubscriptionPolicy(string subscription)
  {
    return Pubsub.Projects.Subscriptions.GetIamPolicy($"{ProjectResource}/subscriptions/{subscription}").Execute();
  }
}