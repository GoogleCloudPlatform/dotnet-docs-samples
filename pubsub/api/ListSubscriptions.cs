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
  // [START list_subscriptions]
  using System;
  using System.Collections.Generic;
  using Google.Apis.Pubsub.v1;
  using Google.Apis.Pubsub.v1.Data;

  public class ListSubscriptionsSample
  {
    public void ListSubscriptions(string projectId)
    {
      PubsubService PubSub = PubSubClient.Create();

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
  }
  // [END list_subscriptions]
}