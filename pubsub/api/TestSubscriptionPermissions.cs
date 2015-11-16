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
  // [START test_subscription_permissions]
  using System;
  using System.Collections.Generic;
  using Google.Apis.Pubsub.v1;
  using Google.Apis.Pubsub.v1.Data;

  public class TestSubscriptionPermissionsSample
  {
    public void TestSubscriptionPermissions(string projectId, string subscriptionName, IList<string> permissions)
    {
      PubsubService PubSub = PubSubClient.Create();

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
  // [END test_subscription_permissions]
}