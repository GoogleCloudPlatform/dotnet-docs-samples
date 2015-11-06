using System;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class GetSubscriptionPolicySample
{
  public void GetSubscriptionPolicy(string projectId, string subscriptionName)
  {
    PubsubService PubSub = PubSubClient.Create();

    Policy policy = PubSub.Projects.Subscriptions.GetIamPolicy(
      resource: $"projects/{projectId}/subscriptions/{subscriptionName}"  
    ).Execute();

    if (policy.Version != null)
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
      Console.WriteLine("Subscription has no policy");
    }
  }
}