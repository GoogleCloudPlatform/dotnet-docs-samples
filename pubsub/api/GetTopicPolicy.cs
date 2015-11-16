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
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

public class GetTopicPolicySample
{
  public void GetTopicPolicy(string projectId, string topicName)
  {
    PubsubService PubSub = PubSubClient.Create();

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
}