using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class SetSubscriptionPolicySample
{
  // hmm.  TODO demonstrate clearly how to create simple policy bindings.

  // NOTE This overwrites any existing policy on the subscription

  // Usage: serviceAccount:myproject@appspot.gserviceaccount.com roles/pubsub.subscriber
  public void SetSubscriptionPolicy(string projectId, string subscriptionName, IDictionary<string, string[]> rolesAndMembers)
  {
    PubsubService PubSub = PubSubClient.Create();

    IList<Binding> bindings = new List<Binding>();

    foreach (var roleName in rolesAndMembers.Keys)
      bindings.Add(new Binding() { Role = roleName, Members = rolesAndMembers[roleName] });

    Policy policy = PubSub.Projects.Subscriptions.SetIamPolicy(
      resource: $"projects/{projectId}/subscriptions/{subscriptionName}",
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