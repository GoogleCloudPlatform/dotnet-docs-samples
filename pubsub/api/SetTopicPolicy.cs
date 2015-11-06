using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class SetTopicPolicySample
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