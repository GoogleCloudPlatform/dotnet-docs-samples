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
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

// TODO Base64
class PublishMessageSample
{
  public void PublishMessage(string projectId, string topicName, string message)
  {
    PubsubService PubSub = PubSubClient.Create();

    message = System.Convert.ToBase64String(
      System.Text.Encoding.UTF8.GetBytes(message)  
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
}