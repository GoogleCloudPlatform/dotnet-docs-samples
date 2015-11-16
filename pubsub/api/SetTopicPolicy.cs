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
  // [START set_topic_policy]
  using System;
  using System.Collections.Generic;

  using Google.Apis.Pubsub.v1;
  using Google.Apis.Pubsub.v1.Data;

  public class SetTopicPolicySample
  {
    public void SetTopicPolicy(string projectId, string topicName, IDictionary<string, string[]> rolesAndMembers)
    {
      PubsubService PubSub = PubSubClient.Create();

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
  }
  // [END set_topic_policy]
}