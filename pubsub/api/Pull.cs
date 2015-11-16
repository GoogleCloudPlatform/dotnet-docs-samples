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

namespace PubSubSample
{

  using System;
  using System.Collections.Generic;
  using Google.Apis.Pubsub.v1;
  using Google.Apis.Pubsub.v1.Data;

  public class PullSample
  {
    public void Pull(string projectId, string subscriptionName)
    {
      PubsubService PubSub = PubSubClient.Create();

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
        List<string> acknowledgeIds = new List<string>();

        foreach (var receivedMessage in receivedMessages)
        {
          string message = System.Text.Encoding.UTF8.GetString(
            System.Convert.FromBase64String(receivedMessage.Message.Data)
          );

          Console.WriteLine(message);

          acknowledgeIds.Add(receivedMessage.AckId);
        }

        PubSub.Projects.Subscriptions.Acknowledge(
          subscription: $"projects/{projectId}/subscriptions/{subscriptionName}",
          body: new AcknowledgeRequest()
          {
            AckIds = acknowledgeIds
          }
        ).Execute();
      }
      else
      {
        Console.WriteLine("There were no messages");
      }
    }
  }
}