using System;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

class GetTopicPolicySample
{
  public void GetTopicPolicy(string projectId, string topicName)
  {
    PubsubService PubSub = PubSubClient.Create();

    Policy policy = PubSub.Projects.Topics.GetIamPolicy(
      resource: $"projects/{projectId}/subscriptions/{topicName}"  
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