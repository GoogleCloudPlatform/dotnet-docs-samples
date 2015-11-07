using System;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class SetSubscriptionPolicySample
{
  // TODO demonstrate clearly how to create simple policy bindings.
  // USAGE serviceAccount:myproject@appspot.gserviceaccount.com roles/pubsub.subscriber
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
  }
}